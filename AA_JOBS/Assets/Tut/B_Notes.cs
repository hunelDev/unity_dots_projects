using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

//Data access in jobs
//joblar giris cikis islemi gerceklestirmemelidir,managed objelere erismemelidir,sadece readonly static fieldlara erismelidir.
//job scheduling islemi ile sadece calisan jobda visible olacak private struct copy yaratiyor.Sonuc olarak job icindeki fieldlarada yapilan degisiklik  sadece job icinde visible olacak.
//ancak eger jobda tanimli bir pointer ya da referans varsa o job disinda da gorunur olabiliyor.

//Unmanaged collections
//unmanaged collectionlar normal managed Unity.Collections lara gore bir takim avantajlari var;
//unmanaged collectionlar Burst-compiled da kullanilabliyor,unmanaged objeler joblarda kullanilabliyor.Managed olanlar safe sekilde kullanlilamiyor,Native- collection typelar job safety check yapabliyor
//thead safety uguluyir joblarda,unmanaged objectler gc ye ihtiyac duymaz ve gc overhead i yok yani.
//downside ise Dispose() cagirmaliyiz eger kullanilmacaksa unmanaged object eger ihmal edilirse memory leake neden olabliyor."disposal safety checks" yinede bu leaklerin cogunu yakalayalbir ve hata firlatabliyor.

//Allocators
//bir unmanaged instantiate edilirken allocator tanimlanmalidir.Farkli allocatorlar farkli sekilde memoriyi organize ederler ve memoroyi track ederler;
//Allocator.Persistent ; En yavas allocatordir.Belirsiz life timesi vardir deallocate etmek icin Dispose() etmek zorundayiz.
//Allocator.Temp ; En hizli allocatordir.Main thread her framede deallocate eder ve tekrar yaratir frame sonunda.Dispose() etmek aslinda hir birsey yapmiyor teknik olarak.Yani yapmaya biliyoruz.
//Allocator.Tempjob ; Orta hizli allocatordir ve joblara gercirilebliyor.
//joblara pass edilen collectionlar thread-safety allocatorlar tarafindan allocate edilmeldiri;

namespace Tut
{
    public struct ExampleJob2:IJob
    {
        public NativeArray<int> Nums;
        public void Execute()
        {
            NativeArray<int> newNums = new NativeArray<int>(Nums, Allocator.Temp);// arasagida pass edilen allocator tempJob olmak zorunda ama job icinde temp veriyoruz bunu
                                                                                  // unutmayalim.Ayrica job icindeki templer otomatk olarak job sonunuda dispose ediliyor.
            Nums[0] = 44;
            Debug.Log(newNums[0]); //burda 0 tutuyor.
        }
    }
    public class B_Notes : MonoBehaviour
    {
        private NativeArray<int> list;
        private JobHandle handle;
        private void Update()
        {
            //Bu unmanaged collection Allocator.Temp ile veriseydi hata verecekti cunku job icine gericiriliyor.
            list =new NativeArray<int>(1,Allocator.TempJob);
            var job= new ExampleJob2{Nums = list};
            handle = job.Schedule();
            
        }

        private void LateUpdate()
        {
            handle.Complete();//burada complate cagirmadan sagida degeri okuyamiyoruz.
            int deneme = list[0];
            list.Dispose(); //allocator.TempJob manuel olarak dispose edilmesi gerekiyor ve  disposal safety checks enable edilmisse allocateden 4 frame sonra hata firlatiyor.
            Debug.Log(deneme);
        }
    }
}