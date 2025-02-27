using Unity.Entities;

//Systems
//Systemler bir entity worldune ait kod kod birimleridir.Maint thread de calisirlar (genellikle her framde bir kere).Normalde Systemler kendi wordlerindeki entitylere ulasirlar ama bu bir kisitlama degildir yani diger world deklerede ulasabilirler
//System ISystem interface den implement edilen bir struct tur.3 key methoda sahip;
//OnUpdate(): Normalde her framede bir kere cagrilir.Ancak bu systemin ait oldugu system grubuna baglidir.
//OnCreate(): OnUpdate first call inda once cagrilir ve system yeniden calistirildiginda cagriliyor.
//OnDestroy(): System destroy eildiginde cagriliyor.
//Not;Bu methodlarin hic birsey yapmayan uygulamari vardir yani bos body olarak tanimlilar ISystem yani ihtiyac duyulmazsa system de ommit edileblirler.mesela OnCreate methodun bodysini bos birakarak butun methodu ommti etmis oluruz.
//Eger systemin Enabled propertysi false olarak set edilirse updatesi skip ediliyor

//System ISysteme ek olarak ISystemStartStop interfacesinden implement edilebliyor.Bu interfacenin methodlari;
//OnStartRunning();OnUpdate methodunun cagirlmadan once cagriliyor gene ve ayrica system re-enabled edildiginde yani Enable property false dan true yapildiginda cagriliyor.
//OnStopRunning(); OnDestroy methodu cagirlmadan once calisiryor ya da Enable property true den false a set edilince cagriliyor.


//System Groups and System Update Order
//Bir worldun sistemleri system grouplarinda organize ediliyorlar.Her system group childrlarin ordered listesine sahiptir.Bu childrlar systemler ya da diger system grouplari olabliyor.
//Default olarak system grouplari onlarin childlarinin OnUpdate methodunu siralanmis bir sekilde cagriyior childa gore.System grouplari bir hierachy ile update orderi belirlerler.
//Bir system group class olarak tanimlanirlar ve ComponentSystemGroupdan inherit edilirler.
//System group update edildiginde default olarak chidlar siralandiklari sekilde update edilirler ancak bu defailt ozelligi update gorubun update methodunu override ederek davranisi degistirilebliyor.
//ornek olarak FixedStepSimulationGroup custom update methdoda sahiptir bu method frame basi sifir veya daha fazla kere childlarini sabit bir update araligini belirlemek icin guncelliyor.Yani framde hic guncellenmeye bilir yada 2 kere guncelleneblir.
//Bir system groupun childlari yeni child eklendiginde tekrar sort ediliyor.
//Childlar stystem grouba UpdateInGroup attribute si ile eklenir.Systemler ve system grouplari bu attribute olmadan default olarak SimulationSystemGroup a ekleniyor.
//UpdateBefore ve UpdateAfter attributeleri gorubun icinde childlar arasinda sira duzenin belirlemek icin kullanilir.Yani o SystemGroubunun iniceki childlar icin.
//ornek olarak mesela FooSystem [UpdateBefore(typeof(BarSystem))] eklenmisse FooSystem BarSystem den once gropta sort edilecek ama 2si ayni SystemGroupda degillerse bir etkisi olmayacak ve bir warning gonderiliyor.
//Eger cildren grouplarinin ordering attributelerin celiski yaratirsa mesela child A BeforeUpdate child B ve childa B de BeforeUpdate child A olarak iseretlenirse group childlari siralnirken hata firlatiliyor.


//ComponentSystemGroup OnUpdate override edilmedigi icin default olarak childrenlar eklendikleri sirayla update edilecekler
public partial class MonsterSystemGroup : ComponentSystemGroup
{
    
}

//MonsterSystemGroup un child systemidir.
[UpdateInGroup(typeof(MonsterSystemGroup))]
public partial struct BroSystem : ISystem, ISystemStartStop
{

    public void OnStartRunning(ref SystemState state)
    {
        
    }
    public void OnStopRunning(ref SystemState state)
    {
        
    }
}


//Creating worlds and systems

   