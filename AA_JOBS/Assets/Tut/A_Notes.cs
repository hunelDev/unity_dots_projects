using System;
using Unity.Jobs;
using UnityEngine;

//Jobs
//joblar bir bir worker thread havuzundan queue ye konulmus olan workleri calisitiyor.Bir worker thread eger isi bitirmise bir soraki bekliyen work u queueden pull ediyor ve jobu calistiriyor.
//Bu jobu calistrma islemini icindkei Execute() methodunu kullanarak yapiyor.
//job struct tanimlanarak yapiliyor ve IJob yada diger interfaceleri implement ederek tanimliyoruz.IJobParallelFor,IJobEntity,IJobChunk
//bir job instanceisin job queuesine koymak icin o instanceden extention method Schedule yi cagiriyoruz.Joblar sadece main thread den scheduled edilebliyor diger threadlerden planlanamiyor yani.


//Job Dependencies
//bir worker thread bir jobu ona bagli olan diger butun execution lar cagirlmadan ve bitirilmeden queueden pull edilmez.
//Yeni scheduled joblarin bir onceki joblar icin beklemesine izin veriyor boylece calisma sirasini ihtiyac duyuldugunda kontrol eder.
//Bir job sadece Scheduled edildikten sonra dependency verilebliyor.
//Schedule() yeni jobu temsil eden JobHandle type donduruyor.Eger bu JobHandle  Schedule a pass edilirse o Schedule JobHandle a depend olmus oluyor.Yani o ilk JobHandle
//worku tamamlamdan depend olan execute edilmez

namespace  Tut
{

    public struct ExampleJob : IJob
    {
        public void Execute()
        {
            Debug.Log("Hello World!");
        }
    }
        
    public class A_Notes:MonoBehaviour
    {
        private void Update()
        {
    
            //job istenen main thread tarafinda calisan her yerde calisitirilabliyor.
            // var job = new ExampleJob{};
            // job.Schedule();

            
            
            var job = new ExampleJob();
           JobHandle handle1= job.Schedule();
           
           var job2 = new ExampleJob();
           JobHandle handle2 = job2.Schedule(handle1); //birinci bitine kadar bekler
           
            
           
           //normalde Schedule sadece bir JobHandler argumet olabliyor ancak JobHandle.CombineDependencies() methodu ile icine multiple handler koyup schedule a pass edebliyoruz.
           JobHandle handle3 = JobHandle.CombineDependencies(handle1, handle2);
           var job3 = new ExampleJob();
           job3.Schedule(handle3);
           
           //Completing jobs
           //Bazi durumlarda bir job Schedule edildikten sonra JobHandle.Complate methodunu cagiriyirouz bu method;jobun butun dependencylerini tamamlar hatta recursive dependencylerin hepsinide tamamlar.
           //jobun execututionu tamamlamasini bekler eger bitirmediyse.
           //Butun job referanslarini queueden kaldirir.
           //Birkere Complate() return edildiginde garanti butun job dependencyleri exectionlar tamamlanir ve queueuden kaldirilir.
           //Jobu tamamlamis bir handledan Complate() cagilirsa hic birsey yapmaz hata da firlatmaz.
           //Schedule gibi sadece main thread den caglilabliyor ve Complate() yi job icinden cagirmak gecersizdir.
           //Bir job schedulingden hemen sonra tamamlanabilir ama complating islemini gercekten ihtiyac oldugu zamana ertlemek en iyisidir.
           //jobun schedulesini ve complationunu bir birinden uzak tutmak main thread ve worker threadlerin bos yere idle olmasini onlemis oluyoruz.
           handle3.Complete();
        }
    }

}
