using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
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
            return View("~/Views/Order/Buyer.cshtml", id);
        }
        [HttpGet("host")]
        public IActionResult Host()
        {
            // get the last order if null then default?
            return View("~/Views/Order/Host.cshtml");
        }
        private async Task CleanUnused(User user)
        {
            var owned = await this.hostOrderDb.FindUnfinished(user.FriendlyId);
            foreach (var item in owned)
            {
                var attached = await this.buyerOrderDb.FindByHostUnfinished(item.Id);
                if (item.Closed < Timing.now() && attached.Count < 1)
                {
                    item.Completed = Timing.now();
                    await this.hostOrderDb.UpdateAsync(item.Id, item);
                }
            }
        }

        [HttpPost("host/add")]
        public async Task<IActionResult> HostAdd(HostOrder hostOrd)
        {
            Debug.WriteLine("run");
            User user = await track();
            await CleanUnused(user);
            var owned = await this.hostOrderDb.FindUnfinished(user.FriendlyId);
            if (owned.Count < 1)
            {
                hostOrd.OwnerUserId = user.FriendlyId;
                hostOrd.Created = Timing.now();
                hostOrd.Closed = Timing.nowAddMinute(hostOrd.Closed);
                hostOrd.Limit = 5;
                await this.hostOrderDb.Insert(hostOrd);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Failed model = new Failed { ErrorMessage = "ขึ้นหน้านี้ กรณียังเคลียรายการต้องซื้อเซทเก่ายังไม่เสร็จ แล้วดันมากดปุ่ม + ในหน้า main" };
                return View("~/Views/Shared/Failed.cshtml", model);
            }
        }
        private async Task<IActionResult> Tick(string? buyOrderId, string ticker, bool state)
        {
            if (buyOrderId != null)
            {
                var buyerOrder = await this.buyerOrderDb.FindById(buyOrderId);
                if (buyerOrder != null)
                {
                    if (ticker == "Buyer")
                        buyerOrder.BuyerChecked = state;
                    else
                        buyerOrder.HostChecked = state;
                    buyerOrder.Completed = (buyerOrder.BuyerChecked && buyerOrder.HostChecked) ? Timing.now() : 0;
                    await this.buyerOrderDb.UpdateAsync(buyOrderId, buyerOrder);
                    var allBuyers = await this.buyerOrderDb.FindByHost(buyerOrder.AttachedHostId);
                    var hostOrder = await this.hostOrderDb.FindById(buyerOrder.AttachedHostId);
                    if (allBuyers.All(o => o.Completed > 0) && hostOrder != null && hostOrder.Id != null && Timing.now() > hostOrder.Closed)
                    {
                        hostOrder.Completed = Timing.now();
                        await this.hostOrderDb.UpdateAsync(hostOrder.Id, hostOrder);
                    }
                    return RedirectToAction(ticker, "List");
                }
            }
            Failed model = new Failed { ErrorMessage = "Cannot find" };
            return View("~/Views/Shared/Failed.cshtml", model);
        }
        [HttpPost("buyer/tick_on")]
        public async Task<IActionResult> BuyerTickOn(Order orderIdOnly)
        {
            return await Tick(orderIdOnly.Id, "Buyer", true);
        }
        [HttpPost("buyer/tick_off")]
        public async Task<IActionResult> BuyerTickOff(Order orderIdOnly)
        {
            return await Tick(orderIdOnly.Id, "Buyer", false);
        }
        [HttpPost("host/tick_on")]
        public async Task<IActionResult> HostTickOn(Order orderIdOnly)
        {
            return await Tick(orderIdOnly.Id, "Host", true);
        }
        [HttpPost("host/tick_off")]
        public async Task<IActionResult> HostTickOff(Order orderIdOnly)
        {
            return await Tick(orderIdOnly.Id, "Host", false);
        }
        [HttpPost("host/close")]
        public async Task<IActionResult> HostClose()
        {
            User a = await track();
            foreach (var hostOrder in await this.hostOrderDb.FindByOwner(a.FriendlyId))
            {
                if (hostOrder.Id == null)
                {
                    continue;
                }
                hostOrder.Closed = Timing.now();
                var allBuyers = await this.buyerOrderDb.FindByHostUnfinished(hostOrder.Id);
                if (allBuyers.Count <= 0)
                {
                    hostOrder.Completed = Timing.now();
                }
                await this.hostOrderDb.UpdateAsync(hostOrder.Id, hostOrder);
            }
            return RedirectToAction("Host", "List");
        }
        [HttpPost("buyer/delete")]
        public async Task<IActionResult> BuyerDel(Order orderIdOnly)
        {
            if (orderIdOnly.Id != null)
                await this.buyerOrderDb.Delete(orderIdOnly.Id);
            return RedirectToAction("Host", "List");
        }
        [HttpPost("buyer/add")]
        public async Task<IActionResult> BuyerAdd(BuyerOrder buyerOrd)
        {
            User user = await track();
            var hostOrd = await this.hostOrderDb.FindById(buyerOrd.AttachedHostId);
            if (hostOrd != null && hostOrd.Id != null)
            {
                var exists = await this.buyerOrderDb.FindByHost(hostOrd.Id);
                var yourBuyerOrders = await this.buyerOrderDb.FindUnfinished(user.FriendlyId);
                if (hostOrd.OwnerUserId == user.FriendlyId)
                {
                    Failed model = new Failed { ErrorMessage = "Why would u order urself? (◣_◢)" };
                    return View("~/Views/Shared/Failed.cshtml", model);
                }
                else if (yourBuyerOrders.Count >= 5)
                {
                    Failed model = new Failed { ErrorMessage = "U ordering too much (◣_◢)" };
                    return View("~/Views/Shared/Failed.cshtml", model);
                }
                else if (hostOrd.Completed > 0 || Timing.now() >= hostOrd.Closed)
                {
                    Failed model = new Failed { ErrorMessage = "Target Order is expired" };
                    return View("~/Views/Shared/Failed.cshtml", model);
                }
                else if (exists.Count >= hostOrd.Limit)
                {
                    Failed model = new Failed { ErrorMessage = "Order exceed" };
                    return View("~/Views/Shared/Failed.cshtml", model);
                }
                else
                {
                    buyerOrd.OwnerUserId = user.FriendlyId;
                    buyerOrd.Created = Timing.now();
                    await this.buyerOrderDb.Insert(buyerOrd);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                Failed model = new Failed { ErrorMessage = "Target no longer exists" };
                return View("~/Views/Shared/Failed.cshtml", model);
            }
        }
    }
}
