﻿using Unity.Entities;

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
//Bu arada Enable property ISystemin propertysi degildri ComponenetSystemGroupun propertysidir.ISystemler onlarin childlari oluyor sonraki baslikta zaten aciliyoruz.Yani group enable true false ediliyor.


//System Groups and System Update Order
//Bir worldun sistemleri system grouplarinda organize ediliyorlar.Her system group childrlarin ordered listesine sahiptir.Bu childrlar systemler ya da diger system grouplari olabliyor.
//Default olarak system grouplari onlarin childlarinin OnUpdate methodunu siralanmis bir sekilde cagriyior childa gore.System grouplari bir hierachy ile update orderi belirlerler.
//Bir system group class olarak tanimlanirlar ve ComponentSystemGroupdan inherit edilirler.
//System group update edildiginde default olarak chidlar siralandiklari sekilde update edilirler ancak bu defailt ozelligi update gorubun update methodunu override ederek davranisi degistirilebliyor.
//ornek olarak FixedStepSimulationSystemGroup custom update methdoda sahiptir bu method frame basi sifir veya daha fazla kere childlarini sabit bir update araligini belirlemek icin guncelliyor.Yani framde hic guncellenmeye bilir yada 2 kere guncelleneblir.
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
//Play modda otomatic boostrapping process default olarak 3 system groupla bir world yaratir;
//InitializationSystemGroup;Unityp player loopun initializetion asamasinin bitmesiyle update edilir.
//SimulationSystemGroup;Unity player loopunun Update asmasinin bitmesyiel Update edilir.
//PresentationSystemGroup;Unity player loopunun PreLateUpdate asamasinin bitmesiyle update edilir.Genellikle rendering kod yerlstriliyormus.
//Automatic bootstrapping her system instancesini ve system group u yaratir.Eger DisableAutoCreation attribute eklenmemisse.Bu system instanceleri default olarak SimulationSystemGroup a ekleniyor eger UpdateInGroup attrubte ile overriden edilmemisse.
//Ornek olarak eger system [UpdateInGroup(typeof(InitializationSystemGroup))] eknemisse SimuatiuonSystemGroup yerine InitializationSystemGroupa eklenecek.

//Autmatic boostrapping asamasi scripting definationla disable edilebliyor;
//UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOSTRAP_RUNTIME_WORLD;deault worldun disable edilmesini sagliyor otomatik boostrapping ismenini.
//UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOSTRAP_EDITOR_WORLD ; editor world de otomatik boostrappingi disable ediyor.
//UNITY_DISABLE_AUTOMATIC_SYSTEM_BOOSTRAP; hem runtime hem de editor wordlu disable ediyor

//Disable edilmisse boostrapping artik bizim kodlarimiz sunlari yaratmakla sorumlu oluyor;
//Creating worlds , Calling World.GetOrCreateSystem<T>() ile system ve system group insancelerini dunyaya eklmek icin, Registration top level systemler (SimulationSystemGroup mesela) ile unity player loopu update etmek icin.
//Alternatif olarak disable etmek yerine customize edebliriz boostrapping logici.Bunun icin ICustomBoostrap implementationlu class yaratarak yapiyoruz bunu


//Time in worlds and systems
//World bir Time Propertye sahiptir bu property TimeData struct return eder;Bu TimeData struct frame delta time ve elapsed time i iceriyor.Time degeri worldun UpdateWorldTimeSystem ile update ediliyor.
//Time valuesi bagzi World methodlari ile degistirilebliyor;
//SetTime; time valuyu set ediyor
//PushTime; gercici olarak time valuesini degisitiriyor
//PopTime; son push time valusini restore ediyor.
//FixedStepSimulationSystemGroup children larinin updatesinden once time value push eder ve updatesinden sonra pop ediyor degeri boylece aslinda yanlis zamani childrlarina gondermis oluyor.


//SystemState
//systemin OnUpdate(),onCreate() ve onDestroy() methodlari SystemState parametersi pass ediyor.SystemState system instacnesinin statesini temsil eder ve onemli property ve methodlar iceriyor.
//Word;systemin wordudur.
//EntityManager;system wordunun entity manageridir
//Dependency; bir JobHandledir job dependencyleri systemler arasinda pass etmek icin kullaniyor.Bu propertyde mesela systemin okudu bilesenlerde bir job varsa bu propertyden alabliyoruz handlesini
//GetEntityQuery;EntityQuery donduruyor.
//GetComponentTypeHandle<T>:ComponentTypeHandle return ediyor.
//GetComponentLookup<T>; ComponentLookup return edyiro.
//EntityQuery,ComponentTypeHnadle ve CompentLookup EntityManagerdan edilnileblisede stateden okumak daha dogrudur diyor.Nedenide diyorki SystemState kullanildiginda component system tracked ile izlenebliyor.
//Buda dogru bir sekilde Dependecny propertynin job dependecyleri pass etmesini sagliyormus.


//SystemAPI
//SystemAPI bir cok yararli static methodlarsa sahiptir.Bu methodlar Word,EntityManager ve SystemStateyi kapsar.
//SystemAPI soruce generatorlare dayanir yani sadece IJobEntity(IJobChunk degil ama) ve system de calisir.Avantaji 2 context de birlkte calisablirmesiymis ve compy-pass de kolaylik sagliyormus.
//Not;Genel kural olarak eger Entity hakkinda bir islem yapacaksak ve kararsizsak once SystemAPI ya bakicaz sonra SystemState bakicaz eger yoksa son olarak EntityManager ve Word a bakicaz.
//SystemAPI Query() methodu sagliyor bu method entityler ve componentler uzerinde foreach loop ile query match ediyor.
