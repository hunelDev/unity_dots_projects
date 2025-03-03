using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

//Accessing entities in jobs
//Job sistemle enitity data islemlerini worker threads ile yapabliyoruz.Entity sistemi 2 interface iceriyormus entitlere ulasabilen job tanimlamak icin.
//IJobChunk; Query ile eslesen bir chunk icin Execute() methodunu cagriyor
//IJobEntity; Query ile eslen her entity icin Execute() methdounu cagriyor.
//IJobEntity kullanima daha uygun olmsada IJobChunk daha kesin kontrol sagliyormus.Genellikle performansli esitmis ayni worklerde.
//Not;IJobEntity aslinda gercek bir job type degildir.Source generation IJobChunk dan impelment ederek IJobEntity structer turetiyor.IJobEntity IJobChunk olarak scheduled edliyor.

//IJobChunk or IJobEntity da workleri bolmek icin coklu threadlere Schedule() yerine ScheduleParallel() cagriliyor.ScheduleParallel kullanildiginda query ile eslesn chunklar ayri batchlere konuyor ve threadlere dagitiliyor.
//Structual changeler bir jobun icinden yapilamiyorlarmis,main thread uzerinde yapilacakmis.Ancak joblar structual change commandlerini kaydebiliyorlarmis EntityCommandBuffer ile ve bu recordlar tekrar main threade oynatiliyormus.


//Sync points
//Bazi operasyonlar main thread uzerinde biraz veya tamamen scheduled jobu tamamlayan 'synchronization pointler' trigger ediyormus.Ornek olarak EntityManager.AddComponent<T> butun scheduled joblari tamamliyor oncelikle T componenetine ulassmak icin.
//benzer sekilde EnitityQuery de ToComponentDataArray<T>(),ToEntityArray() ve ToArchetypeChunkArray() ilk olarak  jobu tamamliyor sonra ayni componentlere query olarak erisiyor.

//Component safety handles
//Native collectionlar gibi her component type her bir world icin iliskili bir job safety handlesi varmis.Bu ayni component type ayni world de erisen 2 job icin safety check ile es zamanli schedule a izin vermiyor.Ornek olarak schedule job
//ile Foo Component type a gerismeye calisitigimzda eger componente erisen bir scheduled job varsa zaten safety check hata firlaticak.Bunu onlemek icin;
//suanki schechedled job yeni job scheduled etilmeden once tamamlanmalidir.
//yada yeni job onceki scheduled joba depend olacak
//Not;Eger 2 job da componente readonly access yapiyorsa es zamanli olarak erisebiliyor.Yani safety check e ReadOnly attrubte ile mark ediligine emin ol diyor.
//DynamicBuffer<T> instancesi kendi safety handlesini tutuyormus;
//DynamicBuffer icerigi ayni buffer component e job ile erisilemiyor.Tamamlanmadan job
//Eger uncomplated joblar read-only access ile buffera erisiyorsa ozaman main thread de read e izin veriyor.


//SystemState.Dependency
//bir jobu system icinde scheduled ettimizde onun bir baska suanda scheduled edilmis joblara bagli olmasi isteriz.Bu joblar yeni joblarla cakisablir.Hatta diger systemdeki joblarla da cakisiyor.Bunu duzenlemek icin SystemState Dependecny
//JobHandle propertyisine sahiptir.Bir systemin updatedinden hemen once;
//Systemin Dependency proeprtysi tamamlanir.
//Ozaman Dependency bu sysmten gibi ayni her hangi bir componente erisen birlestirilmis Denpendency job handles a ataniyor.Ornek olarak Foo ve Bar component typelarina erisen dunyaki bir system dunyadaki diger butun sysmtemlerion Dependencysi
//Foo ve Bar componentlerine ersinenler icin yani systemin Dependencysinde combined ediliyor.
//Ozaman her systemin 2 kurali takip etmesini beklermisiz;
//Systemin update inde scheduled edilen butun joblar updateden once Dependency e attanmis job handle a baglidir.
//Bir systemin update si return edilmeden once,Dependency property update nin icinde scheduled edilen butun joblari iceren bir handle a atanmaliymis.
//Bu iki kurali takip ettigimiz surece bir systemin updatesi diger systemlerde ayni componentlere arismen scheduled edilen joblara bagli olacaklarmis.
//Onemli not;Systemler native collectionlari izlenmiorlarmis sadece Dependency propery component typelar icin hesaplaniyormus.Yani Dependency ye atnmamicaklardir bunu manuel olarak siz ayarlayin ya da diyorki en iyi yolu native collectionu
//bir componentte tutmakmis.


//ComponentLookup<T>
//Entitylerin componentlerine EntityManager ile erisileblir ancak biz genellikle EntityMangeri job un icinde kullanamiyoruz.Bunun yerine ComponentLookup<T> type i kullaniyoruz.Bu type component valuleri get ve set ediyor EntityId ile 
//bunu yapmak icin BufferLookup<T> kullaniyormus.
//Onemli not;Look up entity idle kullanimi performanms kaybina neden oluyor cunku caching islemini missinge neden oluyor.Genel fikir olarak look up kullanamadan yaplirsa oyle yapmakmis.Ama bir cok problemin cozumu random lookupslar gerektiriyomrus
//bu tamaen lookuplari kullanmayin demeke degilmis.Ama dikkatlice kullanmamiz gerekiyormus.
//ComponentLookup<T> ve BufferLookup<T> HasComponent() methoduna sahipmis  true donerse entityde T componenti mevcutdemekmis.TryGetComponent<T> ve TryGetBuffer<T> gene true false donduruyor ayrica output veriyor.
//Not;Entity nin mevcut olup olmadi bastice Exists() methodu EntityStorageInfoLookup uzerinden cagriyabliyormusuz EntityStorageInfoLookup EntityStorageInfo structueri donduruyormus.Bu entity nin chunklarina ve chunkun indexiin refenrasmis.
//Eger bir job ComponentLookup<T> ile read yapacaksa ComponentLookup<T> fieldi ReadOnly olarak olarak isaretlenmelidir ayinisi BufferLookup<T> icinde gecerlidir bunu safety check icin yapiyoruz.
//Paralel Scheduled joblarda ComponentLookup<T> dan component degerlini almak icin fieldlar ReadOnly olarak isaretlenmelidir.Paralel joblarda ComponentLookup<T> ile value set etmeye izin vermiyormus safety check cunku safety garanti degilmis.
//Ancak  ComponentLookup<T> daki safety checki tamamne disable edebliyoruz NativeDisableParallelForRestriction attribute ile  BufferLookup<T> icinde gecerli sadece thread sefetye dikakt edin diyor.
partial struct MySystem : ISystem
{
    [ReadOnly] [NativeDisableParallelForRestriction] ComponentLookup<LocalTransform> look;
    public void OnUpdate(ref SystemState state)
    {
        var entity = state.EntityManager.CreateEntity();
        look = state.GetComponentLookup<LocalTransform>(true);
        var xPos = look.GetRefRO(entity).ValueRO.Position.x;

    }
}

