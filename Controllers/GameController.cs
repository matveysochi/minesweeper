using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using minesweeper.Infrastructure;
using minesweeper.Models.Game;
using minesweeper.Models.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace minesweeper.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private IRecordRepository recordRepository;
        private UserManager<AppUser> userManager;
        public GameController(IRecordRepository recordRepository, UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
            this.recordRepository = recordRepository;
        }
        
        public IActionResult Index() => View();
        public Response State() => GetModel().Response();

        [HttpPost]
        public Response New(FieldTypes? type)
        {
            var model = new GameModel(type ?? FieldTypes.Новичек);
            SaveModel(model);
            return model.Response();
        }

        [HttpPost]
        async public Task<Response> Action(int x, int y, int action)
        {
            var model = GetModel();
            model.Operate(x, y, action);
            if (model.GameEnd && !model.Registred)
            {
                var user = await userManager.GetUserAsync(User);
                recordRepository.AddRecord(new Record(model, user));
                model.Registred = true;
            }
            SaveModel(model);
            return model.Response();
        }

        public async Task<Statistics> Statistics(FieldTypes type)
        {
            var user = await userManager.GetUserAsync(User);
            return recordRepository.GetStatistics(user.Id, type);
        }

        private void SaveModel(GameModel model)
        {
            HttpContext.Session.SetJson("GameModel", model);
        }
        private GameModel GetModel()
        {
            return HttpContext.Session.GetJson<GameModel>("GameModel") ?? new GameModel(FieldTypes.Новичек);
        }
    }
}
