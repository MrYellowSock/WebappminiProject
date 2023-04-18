using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using BookStoreApi.Models;

namespace BookStoreApi.Controllers
{

    [Route("[controller]")]
    public class ListController : DBController
    {
        
        [HttpGet("buyer")]
        public async Task<IActionResult> Buyer()
        {
            Int32? userId = HttpContext.Session.GetInt32("userId");
            
            List<KeyValuePair<HostOrder,BuyerOrder>> list= new List<KeyValuePair<HostOrder, BuyerOrder>>();
            if(userId.HasValue)
            {
                foreach(var buyerOrd in await this.buyerOrderDb.FindUnfinished(userId.Value))
                {
                    var hostOrd = await this.hostOrderDb.FindByBuyer(buyerOrd);
                    if(hostOrd != null)
                    {
                        list.Add(new KeyValuePair<HostOrder, BuyerOrder>(hostOrd,buyerOrd));
                    }
                }
            }

            return View("~/Views/List/Buyer.cshtml",list);
        }
        [HttpGet("host")]
        public async Task<IActionResult> Host()
        {
            Int32? userId = HttpContext.Session.GetInt32("userId");
            List<KeyValuePair<HostOrder,BuyerOrder>> list= new List<KeyValuePair<HostOrder, BuyerOrder>>();
            if(userId.HasValue){
                foreach(var hostOrd in await this.hostOrderDb.FindUnfinished(userId.Value))
                {
                    if(hostOrd.Id == null)
                        continue;
                    foreach(var buyerOrd in await this.buyerOrderDb.FindByHostUnfinished(hostOrd.Id))
                    {
                        list.Add(new KeyValuePair<HostOrder, BuyerOrder>(hostOrd,buyerOrd));
                    }
                }
            }
            return View("~/Views/List/Host.cshtml", list);
        }
    }
}