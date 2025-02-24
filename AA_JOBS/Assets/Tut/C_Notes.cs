using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

//Job Safety Checks
//ayni dataya erismeye calisan 2 job icin cakismalar ya da calisma sirasi belirsizlikleri hatalara neden olabliyor.
//ornek olarak 2 job eger native arrayden degeri okumaya ve yazma calisiyorsa 2 jobdan birinin diger job calismadan tamamlanbmasi gerekiyor.
//job eger arrayi modify etmisse modifikasyon diger job ile cakisabliyor.
//sonuc olarak 2 job arasindaki conflict leri yok etmek icin Schedule ve Complate yapacakmisiz bir jobu digeri baslamadan ya da
//schedule edecekmisiz one jobunu digerune bagli olarak.
//Schedule methodu cagrildigi zaman job safety check ediyor ve bir hata firlatiyor potansiyel bir race condition olabilmesi durumda.

//istisna olarak mesela ayni dataya erismeye calisan 2 job var ve eger bu NativeArray ReadOnly ise sorun olmaz.Yani sadece job icinden okunacak unmaaged datalar eger [ReadOnly] attribute verilmisse sorun yok.
//Bazi duumlarda job safety checki disable eetmek istyebliyoruz.[NativeDisableContainerSafetyRestriction] attribute ile yapabliyoruz.Tabi sadece race condition olmadangindan emin olmaliyiz.
//schuduled joblar tarafindan kullanimda olan native collectionlar eger main threadden okumaya ya da yazma calisirsak hata firlatir ama [ReadOnly] vermissek okuyabliyoruz.
namespace Tut
{
    [BurstCompile]
    struct MyJob : IJob
    {
        public NativeList<int> data;
        [ReadOnly]
        public NativeText str; 

        public void Execute()
        {
            data[0] = 44;
            Debug.Log(str.Value);
            
        }
    }


    public class C_Notes : MonoBehaviour
    {
        private NativeList<int> myList;
        JobHandle handle;
        JobHandle handle2;
        private NativeText t1;
        private NativeText t2;

        private void Update()
        {
            myList = new NativeList<int>(1, Allocator.TempJob) { 123 };


            t1 = new NativeText("job 1", Allocator.TempJob);
            t2 = new NativeText("job 2", Allocator.TempJob);
            
            var job = new MyJob { data = myList, str = t1 };
            var job2 = new MyJob { data = myList,str = t2  };
            handle2 = job2.Schedule();
            handle = job.Schedule(handle2);
        

        }

        private void LateUpdate()
        {
            handle.Complete();
            handle2.Complete();
            print(myList[0]);
            myList.Dispose();
            t1.Dispose();
            t2.Dispose();
            
        }


    }
}