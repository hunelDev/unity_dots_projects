using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

//Parallel Jobs
//work processing islemlerini multple threadler boyunca boluyor.IJobParallelFor dan implement ediyoruz.
//job calisirken Execute() methodu count times kadar cagriliyor yani ilk parametresi Schedule da.Yani 0 dan counte kadar index icinde utuyor bu degeri.
//indexler batch size kadar bolunuyor ve bolunen batchler worker threadler bu batchleri queueuden aliyor.
//Effective sekilde bolunmus batchler es zamanli olarak ayri threadler de isleniyor,Ancak individual batchdeki butun indexlerin hepsi single threadde birlikte isleniyorlar.
//Mesla soyle dusunelim array.lengthi 250 ise ve bizim batch size 100 olsun.Boyle bir durumda batchlerin 1.si 0-99 arasindaki indexleri threadde isliyor bir baska threadde 100-199 arasindaki indexler isleniyor ve geriye kalan 200-250 arasindaki
//indexler de baska bir threadde o 3. batchi alarak islenecek bu batchlerde o threade mesela 100 kere loop iterate edlerek isleniyor.Daha fazla threade bolmek istiyorsak batch size kucultmemiz lazim.
//not olarak diyorki iyi bir batch yasina bolmek bir bilim degildir.Yani batchi kucuk tutup threadlerin birlkte daha hizli is tamamlamasiin sagliyabliyoruz.Ama diyorki bu sistem yuke neden oluyor tabiki

namespace Tut
{
    [BurstCompile]
    struct Parallel : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<int> Nums;
        public void Execute(int index)
        {
            Nums[index] *= Nums[index]; //burada bilmemiz gereken en onemli sey su buraya mesela Nums[0] veremeyiz cunku batch indexleri threadlere bolundukten sonra bu 0 indexi sadece tek threade isnelecei icin
            //the safety check hata firlatiyor eger bunu soyle yapsaydik mesela;
            //Nums[index] = Nums[1] + 1; //burdaki gibi yapamiyoruz batch designed range disinda olamaz erislmeye calisilan array indexi burdaki 1 sadece bir threadde desing edilmistir.
            
            //ama [ReadOnly] olarak vermis olsaydik arrayi ya da listeyi sorun yoktu ya da safety checki kaldirmak icin gene [NativeDisableParallelForRestriction] i kullanicaz
            Debug.Log(Nums[5] + "");
        }
    }
    
 
    public class D_Notes : MonoBehaviour
    {
        JobHandle handle;
        public NativeArray<int> myArray;
        
        private void Start()
        {
            myArray = new NativeArray<int>(interator(), Allocator.TempJob);
            var job = new Parallel { Nums = myArray };
            handle = job.Schedule(myArray.Length, 100);
            handle.Complete();
            
            Debug.Log(myArray[44]);
            myArray.Dispose();
        }

        int[] interator(int count = 250)
        {
            int[] array = new int[count];
            for (int i = 0; i< count; i++)
            {
                array[i] = i;
            }

            return array;
        }

        
    }
}