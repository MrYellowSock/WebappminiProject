using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Models;
using BookStoreApi.Util;

namespace BookStoreApi.Controllers
{
    public class HomeController : DBController
    {
        public async Task<IActionResult> Index()
        {
            List<KeyValuePair<int,HostOrder>> hosts = new List<KeyValuePair<int, HostOrder>>();
            long timeNow = Timing.now();
            foreach(HostOrder h in await this.hostOrderDb.FindAllNonClosed(timeNow))
            {
                if(h.Id == null)
                    continue;
                int Used = (await this.buyerOrderDb.FindByHost(h.Id)).Count;
                hosts.Add(new KeyValuePair<int, HostOrder>(Used, h));
            }
            
            Int32? userId = HttpContext.Session.GetInt32("userId");
            if(userId.HasValue)
            {
                List<string> alias = ( await this.hostOrderDb.FindByOwner(userId.Value) )
                    .Select(ord=>ord.OwnerName)
                    .Concat(( await this.buyerOrderDb.FindByOwner(userId.Value) ).Select(ord=>ord.OwnerName))
                    .Distinct()
                    .ToList();

                return View(new HomeIndex(hosts,alias,userId.Value));
            }
            return View(new HomeIndex(hosts, new List<string>()));

        }
    }
}