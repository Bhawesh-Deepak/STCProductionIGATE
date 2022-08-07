namespace STCAPI.Controllers.Report
{
    //[Route("api/[controller]/[action]")]
    //[EnableCors("AllowAnyOrigin")]
    //[ApiController]
    //private class ReportController : ControllerBase
    //{
    //    private readonly IGenericRepository<NewReportModel, int> _INewReportModelRepository;
    //    private readonly IGenericRepository<PortalMenuMaster, int> _IPortalMenuRepository;

    //    /// <summary>
    //    /// Inject the required service to the constructor
    //    /// </summary>
    //    /// <param name="newReportModelRepo"></param>
    //    /// <param name="portaMenuRepo"></param>
    //    public ReportController(IGenericRepository<NewReportModel, int> newReportModelRepo, IGenericRepository<PortalMenuMaster, int> portaMenuRepo)
    //    {
    //        _INewReportModelRepository = newReportModelRepo;
    //        _IPortalMenuRepository = portaMenuRepo;
    //    }

    //    [HttpGet]
    //    [Produces("application/json")]
    //    [Consumes("application/Json")]
    //    public async Task<IActionResult> GetMainStream()
    //    {
    //        var responseData = await _IPortalMenuRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

    //        List<string> mainStreamList = new List<string>();
    //        responseData.TEntities.ToList().ForEach(item =>
    //        {
    //            if (!mainStreamList.Any(x => x.Contains(item.MainStream)))
    //            {
    //                mainStreamList.Add(item.MainStream);
    //            }

    //        });

    //        return Ok(mainStreamList);
    //    }

    //    [HttpGet]
    //    [Produces("application/json")]
    //    [Consumes("application/json")]
    //    public async Task<IActionResult> GetSubStream(string mainStream)
    //    {
    //        var responseData = await _IPortalMenuRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
    //        var subStreamList = responseData.TEntities.Where(x => x.MainStream == mainStream).ToList();

    //        List<string> subStreams = new List<string>();

    //        subStreamList.ForEach(item =>
    //        {
    //            if (!subStreams.Any(x => x.Contains(item.Stream ?? string.Empty)))
    //            {
    //                subStreams.Add(item.Stream);
    //            }

    //        });

    //        return Ok(subStreams);
    //    }

    //    [HttpPost]
    //    [Produces("application/json")]
    //    [Consumes("application/json")]
    //    public async Task<IActionResult> CreateNewReport(NewReportViewModel model)
    //    {
    //        var dbModel = new NewReportModel();
    //        dbModel.MainStream = model.Main_Stream;
    //        dbModel.Stream = model.Stream;
    //        dbModel.ReportName = model.Report_Name;
    //        dbModel.ReportNumber = model.Report_Number;
    //        dbModel.ReportLongName = model.Report_Long_Name;
    //        dbModel.ReportShortName = model.Report_Short_Name;
    //        dbModel.ReportDescription = model.Report_Description;
    //        dbModel.IsActive = true;

    //        NewReportModel[] dbModelArray = { dbModel };
    //        var response = await _INewReportModelRepository.CreateEntity(dbModelArray);
    //        return Ok(response);
    //    }

    //    [HttpGet]
    //    [Produces("application/json")]
    //    [Consumes("application/json")]
    //    public async Task<IActionResult> GetReportDetails()
    //    {
    //        List<NewReportViewModel> models = new List<NewReportViewModel>();
    //        var dbResponseModels = await _INewReportModelRepository.GetAllEntities(x=>x.IsActive && !x.IsDeleted);

    //        dbResponseModels.TEntities.ToList().ForEach(item =>
    //        {
    //            var model = new NewReportViewModel();
    //            model.Id = item.Id;
    //            model.Main_Stream = item.MainStream;
    //            model.Stream = item.Stream;
    //            model.Report_Name = item.ReportName;
    //            model.Report_Number = item.ReportNumber;
    //            model.Report_Long_Name = item.ReportLongName;
    //            model.Report_Short_Name = item.ReportShortName;
    //            model.Report_Description = item.ReportDescription;

    //            models.Add(model);
    //        });

    //        return Ok(models);
    //    }

    //    [HttpGet]
    //    public async Task<IActionResult> DeleteReport(int id)
    //    {
    //        var response = await _INewReportModelRepository.GetAllEntities(x => x.Id == id);

    //        if (response.TEntities.Any())
    //        {
    //            response.TEntities.ToList().ForEach(item =>
    //            {
    //                item.IsActive = false;
    //                item.IsDeleted = true;
    //            });

    //            var deleteResponse = await _INewReportModelRepository.UpdateEntity(response.TEntities.First());

    //            return Ok(deleteResponse);
    //        }

    //        return BadRequest($"Invalid Report Id {id}");

    //    }

    //    [HttpGet]
    //    [Produces("application/json")]
    //    [Consumes("application/json")]
    //    public async Task<IActionResult> GetReportById(int id)
    //    {
    //        var model = new NewReportViewModel();

    //        var responseData = await _INewReportModelRepository.GetAllEntities(x => x.Id == id);
    //        if (responseData.TEntities.Any())
    //        {
    //            responseData.TEntities.ToList().ForEach(x =>
    //            {
    //                model.Id = x.Id;
    //                model.Stream = x.Stream;
    //                model.Main_Stream = x.MainStream;
    //                model.Report_Name = x.ReportName;
    //                model.Report_Long_Name = x.ReportLongName;
    //                model.Report_Short_Name = x.ReportShortName;
    //                model.Report_Long_Name = x.ReportLongName;
    //                model.Report_Description = x.ReportDescription;
    //                model.Report_Number = x.ReportNumber;

    //            });

    //            return Ok(model);
    //        }
    //        return BadRequest($"Invalid Reeport Id {id}");
    //    }

    //    [HttpPut]
    //    [Produces("application/json")]
    //    [Consumes("application/json")]
    //    public async Task<IActionResult> UpdateReportDetails(NewReportViewModel model)
    //    {


    //        var response = await _INewReportModelRepository.GetAllEntities(x => x.Id == model.Id);

    //        if (response.TEntities.Any())
    //        {
    //            response.TEntities.ToList().ForEach(item =>
    //            {
    //                item.IsActive = false;
    //                item.IsDeleted = true;
    //            });

    //            var deleteResponse = await _INewReportModelRepository.UpdateEntity(response.TEntities.First());

    //            var dbModel = new NewReportModel();

    //            dbModel.MainStream = model.Main_Stream;
    //            dbModel.Stream = model.Stream;
    //            dbModel.ReportName = model.Report_Name;
    //            dbModel.ReportNumber = model.Report_Number;
    //            dbModel.ReportLongName = model.Report_Long_Name;
    //            dbModel.ReportShortName = model.Report_Short_Name;
    //            dbModel.ReportDescription = model.Report_Description;
    //            dbModel.IsActive = true;

    //            NewReportModel[] dbModelArray = { dbModel };
    //            var dbResponse = await _INewReportModelRepository.CreateEntity(dbModelArray);

    //            return Ok(dbResponse);

    //        }

    //        return BadRequest("Id not found ..");
    //    }
    //}
}