using maplestory.io.Services.MapleStory;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PKG1;
using SixLabors.ImageSharp;
using System;
using System.Linq;
using WZData;
using WZData.ItemMetaInfo;
using WZData.MapleStory.Items;

namespace maplestory.io.Controllers
{
    [Produces("application/json")]
    [Route("api/{region}/{version}/item")]
    public class ItemController : Controller
    {
        private readonly IItemFactory itemFactory;
        private readonly JsonSerializerSettings serializerSettings;

        public ItemController(IItemFactory factory)
        {
            itemFactory = factory;

            IgnorableSerializerContractResolver resolver = new IgnorableSerializerContractResolver();
            resolver.Ignore<ItemNameInfo>(a => a.Info);
            resolver.Ignore<ItemType>(a => a.HighItemId);
            resolver.Ignore<ItemType>(a => a.LowItemId);

            serializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = resolver,
                Formatting = Formatting.Indented
            };
        }

        [FromRoute]
        public Region region { get; set; }
        [FromRoute]
        public string version { get; set; }
        [Route("")]
        [Route("list")]
        [HttpGet]
        [ProducesResponseType(typeof(ItemNameInfo[]), 200)]
        public IActionResult List(
            [FromQuery] uint startPosition = 0, 
            [FromQuery] uint? count = null, 
            [FromQuery] string overallCategoryFilter = null, 
            [FromQuery] string categoryFilter = null, 
            [FromQuery] string subCategoryFilter = null, 
            [FromQuery] int? jobFilter = null, 
            [FromQuery] bool? cashFilter = null,
            [FromQuery] int? minLevelFilter = null, 
            [FromQuery] int? maxLevelFilter = null, 
            [FromQuery] int? genderFilter = null, 
            [FromQuery] string searchFor = null
        )
            => Json(itemFactory.GetWithWZ(region, version).GetItems(startPosition, count, overallCategoryFilter, categoryFilter, subCategoryFilter, jobFilter, cashFilter, minLevelFilter, maxLevelFilter, genderFilter, searchFor), serializerSettings);

        [Route("category")]
        [HttpGet]
        [ProducesResponseType(typeof(string[]), 200)]
        public IActionResult GetCategories() => Json(itemFactory.GetWithWZ(region, version).GetItemCategories());

        [Route("category/{overallCategory}")]
        [HttpGet]
        [ProducesResponseType(typeof(ItemNameInfo), 200)]
        public IActionResult ListByCategory(string overallCategory)
            => Json(itemFactory.GetWithWZ(region, version).GetItems().Where(c => c.TypeInfo.OverallCategory.Equals(overallCategory, StringComparison.CurrentCultureIgnoreCase)), serializerSettings);

        [Route("bulk/{ids}")]
        [HttpGet]
        [ProducesResponseType(typeof(Tuple<int, IconInfo, ItemNameInfo>[]), 200)]
        public IActionResult GetItemIconName(string ids)
        {
            int[] itemIds = ids
                .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Where(c => int.TryParse(c, out int blah))
                .Select(c => int.Parse(c))
                .ToArray();

            return Json(itemFactory.GetWithWZ(region, version).BulkItemInfo(itemIds));
        }

        [Route("{itemId}")]
        [HttpGet]
        [ProducesResponseType(typeof(MapleItem), 200)]
        public IActionResult itemSearch(int itemId)
        {
            MapleItem eq = itemFactory.GetWithWZ(region, version).search(itemId);
            return Json(eq);
        }

        [Route("{itemId}/icon")]
        [HttpGet]
        [Produces("image/png")]
        public IActionResult itemIcon(int itemId)
        {
            Image<Rgba32> icon = null;
            if (itemFactory.GetWithWZ(region, version).DoesItemExist(itemId) && (icon = itemFactory.GetWithWZ(region, version).GetIcon(itemId)) != null)
                return File(icon.ImageToByte(Request), "image/png");
            return NotFound("Item does not have an icon or a default effect");
        }

        [Route("{itemId}/iconRaw")]
        [HttpGet]
        [Produces("image/png")]
        public IActionResult itemIconRaw(int itemId)
        {
            Image<Rgba32> icon = null;
            if (itemFactory.GetWithWZ(region, version).DoesItemExist(itemId) && (icon = itemFactory.GetWithWZ(region, version).GetIconRaw(itemId)) != null)
                return File(icon.ImageToByte(Request), "image/png");
            return NotFound("Item does not have an icon or a default effect");
        }

        [Route("{itemId}/name")]
        [HttpGet]
        [Produces("text/json")]
        public IActionResult itemName(int itemId)
        {
            MapleItem eq = itemFactory.GetWithWZ(region, version).GetWithWZ(region, version).search(itemId);
            return Json(eq.Description);
        }
    }
}