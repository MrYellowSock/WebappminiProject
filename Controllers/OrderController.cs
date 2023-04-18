using Microsoft.AspNetCore.Mvc;
using System.Data; using System.Diagnostics;
using BookStoreApi.Models;
using BookStoreApi.Services;
using BookStoreApi.Util;

namespace BookStoreApi.Controllers
{

    [Route("[controller]")]
    public class OrderController : DBController
    {
        [HttpGet("buyer/{id:length(24)}")]
        public IActionResult Buyer(string id)
        {
            return View("~/Views/Order/Buyer.cshtml",id);
        }
        [HttpGet("host")]
        public IActionResult Host()
        {
            // get the last order if null then default?
            return View("~/Views/Order/Host.cshtml");
        }
        [HttpPost("host/add")]
        public async Task<IActionResult> HostAdd(HostOrder hostOrd)
        {
            Debug.WriteLine("run");
            User user = await track();
            var owned = await this.hostOrderDb.FindUnfinished(user.FriendlyId);
            if(owned.Count < 1){
                hostOrd.OwnerUserId = user.FriendlyId;
                hostOrd.Created = Timing.now();
                hostOrd.Closed = Timing.nowAddMinute(hostOrd.Closed);
                hostOrd.Limit = 5;
                await this.hostOrderDb.Insert(hostOrd);
                return View("~/Views/Order/Success.cshtml");
            }
            else{
                Failed model = new Failed { ErrorMessage = "YOu have unfinished stuffs" };
                return View("~/Views/Shared/Failed.cshtml", model);
            }
        }
        private async Task<IActionResult> Tick(string? buyOrderId, string ticker)
        {
            if(buyOrderId != null){
                var buyerOrder = await this.buyerOrderDb.FindById(buyOrderId);
                if(buyerOrder != null){
                    if(ticker == "Buyer")
                    {
                        buyerOrder.BuyerChecked = true;
                    }
                    else {
                        buyerOrder.HostChecked = true;
                    }
                    if(buyerOrder.BuyerChecked && buyerOrder.HostChecked)
                    {
                        buyerOrder.Completed = Timing.now();
                    }
                    await this.buyerOrderDb.UpdateAsync(buyOrderId,buyerOrder);
                    var allBuyers = await this.buyerOrderDb.FindByHost(buyerOrder.AttachedHostId);
                    var hostOrder = await this.hostOrderDb.FindById(buyerOrder.AttachedHostId);
                    if( allBuyers.All(o=>o.Completed > 0) && hostOrder != null && hostOrder.Id != null && Timing.now() > hostOrder.Closed)
                    {
                        hostOrder.Completed = Timing.now();
                        await this.hostOrderDb.UpdateAsync(hostOrder.Id,hostOrder);
                    }
                    return RedirectToAction(ticker,"List");
                }
            }
            Failed model = new Failed { ErrorMessage = "Cannot find" };
            return View("~/Views/Shared/Failed.cshtml", model);
        }
        [HttpPost("buyer/tick")]
        public async Task<IActionResult> BuyerTick(Order orderIdOnly)
        {
            return await Tick(orderIdOnly.Id,"Buyer");
        }
        [HttpPost("host/tick")]
        public async Task<IActionResult> HostTick(Order orderIdOnly)
        {
            return await Tick(orderIdOnly.Id,"Host");
        }
        [HttpPost("host/close")]
        public async Task<IActionResult> HostClose()
        {
            User a = await track();
            foreach(var hostOrder in await this.hostOrderDb.FindByOwner(a.FriendlyId))
            {
                if(hostOrder.Id == null){
                    continue;
                }
                hostOrder.Closed = Timing.now();
                var allBuyers = await this.buyerOrderDb.FindByHost(buyerOrder.AttachedHostId);
                if (allBuyers.All(o=>o.Completed > 0))
                {
                    hostOrder.Completed = Timing.now();
                }
                await this.hostOrderDb.UpdateAsync(hostOrder.Id,hostOrder);
            }
            return RedirectToAction("Host","List");
        }
        [HttpPost("buyer/delete")]
        public async Task<IActionResult> BuyerDel(Order orderIdOnly)
        {
            if(orderIdOnly.Id != null)
                await this.buyerOrderDb.Delete(orderIdOnly.Id);
            return RedirectToAction("Host","List");
        }
        [HttpPost("buyer/add")]
        public async Task<IActionResult> BuyerAdd(BuyerOrder buyerOrd)
        {
            User user = await track();
            var hostOrd = await this.hostOrderDb.FindById(buyerOrd.AttachedHostId);
            if(hostOrd != null && hostOrd.Id != null)
            {
                var exists = await this.buyerOrderDb.FindByHost(hostOrd.Id);
                if(hostOrd.OwnerUserId == user.FriendlyId){
                    Failed model = new Failed { ErrorMessage = "Why would u order urself?" };
                    return View("~/Views/Shared/Failed.cshtml", model);
                }
                else if(hostOrd.Completed > 0 || Timing.now() >= hostOrd.Closed)
                {
                    Failed model = new Failed { ErrorMessage = "Target Order is expired" };
                    return View("~/Views/Shared/Failed.cshtml", model);
                }
                else if(exists.Count >= hostOrd.Limit){
                    Failed model = new Failed { ErrorMessage = "Order exceed" };
                    return View("~/Views/Shared/Failed.cshtml", model);
                }
                else{
                    buyerOrd.OwnerUserId = user.FriendlyId;
                    buyerOrd.Created = Timing.now();
                    await this.buyerOrderDb.Insert(buyerOrd);
                    return View("~/Views/Order/Success.cshtml");
                }
            }
            else{
                Failed model = new Failed { ErrorMessage = "Target no longer exists" };
                return View("~/Views/Shared/Failed.cshtml", model);
            }
        }
    }
}