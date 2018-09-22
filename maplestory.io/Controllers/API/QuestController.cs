using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace maplestory.io.Controllers.API
{
    [Route("api/{region}/{version}/quest")]
    public class QuestController : APIController
    {
        [Route("")]
        [HttpGet]
        public IActionResult List(
            [FromQuery] string searchFor = null,
            [FromQuery] int startPosition = 0,
            [FromQuery] int? count = null
        ) => Json(QuestFactory.GetQuests(searchFor, startPosition, count));

        [Route("{questId}")]
        [HttpGet]
        public IActionResult GetQuest(int questId)
        {
            var quest = QuestFactory.GetQuest(questId);
            if (quest == null) return NotFound();
            return Json(quest);
        }

        [Route("category")]
        [HttpGet]
        public IActionResult GetQuestCategories()
            => Json(WZ.QuestAreaNames.OrderBy(c => c.Key));

        [Route("category/{category}")]
        [HttpGet]
        public IActionResult GetQuestInCategory(int category)
            => WZ.QuestAreaLookup.TryGetValue(category, out var inCategory) ? Json(inCategory.Select(c => new { id = c.Item1, name = c.Item2 }).OrderBy(c => c.id)) : (IActionResult)NotFound();

        [Route("{questId}/name")]
        [HttpGet]
        public IActionResult GetName(int questId)
        {
            var questData = QuestFactory.GetQuest(questId);
            if (questData == null) return NotFound();
            return Json(new
            {
                id = questData.Id,
                name = questData.Name
            });
        }
    }
}