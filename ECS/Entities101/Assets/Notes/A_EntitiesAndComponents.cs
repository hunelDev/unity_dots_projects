// Entities and components
// Entity gameobjectin lightweight unmanaged alternatifi olarak cikmistir.Benzerler ama farkli ozellikleri var;
// Entitty gameobject gibi managed object degildir ancak unique identifier numarasi vardir.
// Enitity componentde MonoBehaviour ve eventlerinin karsiligi yoktur. OnUpdate Start gibi
// Entitiylerin methodlari olabiliyor ama nerdeyse hic yapmiyoruz.
// Bir enitity sadece ayni componenetten bir taneye sahip olabliyor.
// Entity yerlesik bir parenting kavramina sahip degildir.Bunun yerine  Parent  componeneti varmis ve bu componenette baska bir entity nin referansini iceriyormus ve bu yapi transform hierarcysine izin veriyormus enitity icin.

// Basit bir entity componeneti struct ile tanimlaniyor ve IComponentData implement ediyor.

using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

public struct Health : IComponentData
{
    public int HitPoints;
    public float ArmourRating;

}

//IComponenetData structunun unmaged olmasi gerekiyor.Yani managed type icermeyecek Ozeillikle icereblecegi typelar;
//Blittable types(bunlar common typelardir bildigimiz hem managed olablilen hem de unmanaged karsiliklari olablilen typlar int,double,float managed System.Single vs.. oluyor yani)
//bool,char
//BlobAssetReference<T> Blob sturucteru temsil ediyor (Binary Large Objects) 
//Collections.FixedString fixed size character bufferi
//Collections.FixedLists
//Fixed Array unsafe context olacak sadece   int* ptr = stackalloc int[10];
//Diger struct typelar tabi icinde managed type icermeyenler


//Entity Worlds and EntityManagers
//Bir world entity collectionudur.Bir entitynin idsi sadece onun worldunde uniquetir.Ayni id ye sahip baska bir world deki entityle alakasi yokltur yani.
//Wolrdler ayrica kendilerinin kodlbirimleri olan sistem kumelerinie sahiptir.Bu systemler main threadde calisirlar ve genellikle her frame de 1 kere calisiyorlar.
//Worldun entititylerine normalde sadece wordlun sistemi ile erisilebliyor,ve sistemlerden ayrica joblar scheduled edilebliyor.(ama bu zorunlu bir kisitlama degil diyor).

//Bir dunyaki entityler created,destroy ve modified ismeini wordlun sahip oldugu EntityManager ile yapabliyor ve bu manager methodlar iceriyor bunlari yapamk icin;

partial struct DenemeSystem : ISystem
{
    private NativeArray<Entity> entities;

    [BurstCompile]
    void OnCreate(ref SystemState state)
    {
        EntityArchetype archetype = new(); //parametre ollarak archetype verebliyoruz entitiy yaratirken;
        Entity newEntity = state.EntityManager.CreateEntity(archetype); //yeni entitiy yaratiyor
        state.EntityManager.Instantiate(newEntity); //yeni entity yarat ve srcEntity olarak yani kaynak olarak diger entityin componentlerini kopyalar
        state.EntityManager.DestroyEntity(newEntity); //destroy ediyor entityi
        state.EntityManager.AddComponent<Health>(newEntity); //entitye yeni component ekle T typeda
        state.EntityManager.AddComponentData(newEntity, new Health()); //Buda yani islemi yapiyor Componenet ekledik
        state.EntityManager.RemoveComponent<Health>(newEntity);
        state.EntityManager.HasComponent<Health>(newEntity); //geriye bool donduyuro ve componenet o entityde varmi
        Health health = state.EntityManager.GetComponentData<Health>(newEntity); //notta GetComponent olani var ama yeni versionda yok sanirim
        state.EntityManager.SetComponentData(newEntity, new Health { HitPoints = health.HitPoints - 5 }); //entity componentinin valularini overwrite ediyor.

    }

    void OnDestroy(ref SystemState state)
    {
        entities.Dispose();
    }
}


//Archetypes
//Bir archetype component typelarin ozellestirlmis unique combinasyonunu temsil eder.Belli component tip kumelerini iceren butun enitityler birlikte ayni archetype da depolaniyorlar.
//Bir dunyadaki component A,B ve C yi iceren entityler bir archetypeda birlikte tutulurolar
//A ve B yi iceren ama C componentini icermeyenler ikinci bir archetypeda tutuluyor.
//B ve D componentli olanlar ise 3. bir archetypeda tutuluyor.
//Effective sekilde bir componenti entitiye eklemek ya da kaldirmak onun ait oldugu archetypesini degisitiriyor.Bu da EntityManagerin entitiyi yeni bir archetype atasimasini gerektirir.
//Mesela x y ve z componentlerine sahip bir entityden y kaldirilirsa x ve z componentlerinin oldugu archetype a tasir ve x ve z degelerini kopyalar yoksa yeni archetype olusturuyor.

//bu archetype arasinda cok sayida entity tasima islemileri maliyetli olabliyormus not olarak vermis
//EntityManager bu olsutmra islemini otomatik olarak yapiyor bizim entitiy olsuturdugumuz gibi tammen archetype bos olsa bile yok edilmiyormus world destroy edilene kadar.


//Chunks