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
//Archetype enityleri 16KB lik belleklerde saklanirlar bu belleklere chunk yani parca deniyor.Maximum 128 enitity tutabliyor icinde.Eger 16kb/128 durumunu asan bir entity olursa 128den daha az entity tutucak.
//Chunklarin arraylerinde entitylerinin idleri ve her type componenet tutuluyor.Ornek olarak A ve B type componenete sahip archetypelardaki chunklar bir 3 ayri array tutucak bir array entity ID leri,
//2. array A componentleri ve 3. array B componentleri icin tutucak
//ilk entitynin id ve componentleri chunkda 0 indexi ile tutulcak.2. entity 1. indexde ve boyle devem ediyor.
//chunkda arrayler siki paketler halinde tutuluyor.Yeni bir entity eklendiginde ilk free array indexinde depolaniyor.Eger bir entity chunkdan kaldirilirsa bu islem oluyor cunku entity yok destroy edileblir ya da archetype degisebliyor.
//Bu durumda son entity boslugu doldurmak icin oraya tasiniyor.
//chunk olsturmak ya da destroy etmek EnitityManager tarafindan handle ediliyor.
//EntityManager eger bir chunk fullse ve ona yeni bir entity o archetypa eklenmek isteniyorsa olusutuyor.
//EntityManager eger son entity chunkdan kaldirilmisa destory ediyor.
//EntityManager in yaptigi add,remove ve move operasyonlarina structural change deniyor.Bu tur degisikler sadece main thread de yapilabliyor yani joblarda yapilamiyor ileride EntityCommandBuffer konusunu konusucakmisiz bu bu sorunu cozuyormus.


//Queries
//Bir EntityQuery effective bir sekilde belli component kumlerini icinde barindiran butun entityleri buluyor.Mesela type A ve type B iceren butun entityler icin bakiyor, sonra Query archetypelarin chunklarini bir araye getiriyor.
//Bu chunklarda da diger component typelarin ne oldugnun bir onemi yok getiriyor mesela C type componentte olablir bu archetypelarda.Boylece query A ve B ye sahip entitylerle eslesiyor ama ayni zamanda mesela C ye sahip olanlada eslesiyor.
//Not; query de eslesen archetypelar, yeni bir archetype world e eklenene kadar cheche ediliyor.Boylece chaching ile queryler cok daha ucuzdur.
//Ayrica query mesela component A ve B olan ama C olmayan eslesmer icin arama da yapablir yani mesela icinde C olan archetypelari exclude ediyor.



//Enity ID
//Bir entity ID entity struct tarafindan temsil edilen 2 int den meydana gelir bunlar index ve version.
//ID ile entitylere bakmak icin EntityManager metadata arrayde tutuyor gerekli datalari.Entity nin indexi metadata array slotununu belirtiyor ve slot enitity nin tutuldugu chunk a bir pointer tutuyor.Ayrica chunkda entityinin indexi var
//Eger index de bir entity yoksa, indexdeki chunk pointer nulldur.
//ornek olarak eniny metadata array = [chunk*,index in chunk,version number],[chunk*,index in chunk,version number],[chunk*,index in chunk,version number] boyle boyle gidiyor
//her world kendi entitylerini buyuk single bir arrayden izler.Entity ID index = metadata arrayi slotundaki indexi, Entity ID Version = o index slotundaki entity nin her destroyed edildiginde arttirilmis degeridir.

//entity version numarasi entitynin destroyed edildikten sonra entitiy indexinin tekrar kullanilmasina izin veriyor.Bir entity destory edildiginde version numarasi o indexde arttirilir.Eger id nin version numarasi,arrayde tutulan ile
//eslesmiyorsa kimlik daha once yok edilmis ya da hic var olmamis olablirmis.



//Tag components




























