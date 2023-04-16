using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using BookStoreApi.Models;
using BookStoreApi.Services;
using BookStoreApi.Util;

namespace BookStoreApi.Controllers
{
    public abstract class DBController : Controller
    {
        protected readonly HostOrderService hostOrderDb ;
        protected readonly BuyerOrderService buyerOrderDb ;
        protected readonly UserService userService ;

        public async Task<User> track() {

            Int32? userId = HttpContext.Session.GetInt32("userId");
            User? userNow;
            if(userId.HasValue)
            {
                userNow = await this.userService.FindByFId(userId.Value);
                if(userNow != null)
                {
                    userNow.LastActive = Timing.now();
                    await this.userService.Update(userId.Value,userNow);
                    return userNow;
                }
            }
            User? userMax = await this.userService.FindMaxFId();
            Int32 uid = 0;
            if(userMax != null)
            {
                uid = userMax.FriendlyId + 1;
            }
            User newUser = new User();
            newUser.FriendlyId = uid;
            newUser.Created = Timing.now();
            newUser.LastActive = newUser.Created;
            await this.userService.Insert(newUser);
            HttpContext.Session.SetInt32("userId", uid);
            return newUser;
        }
        public DBController()
        {
            DBService dbs = new DBService("mongodb://localhost:27017", "foody");
            this.hostOrderDb = new HostOrderService(dbs.GetCollection<HostOrder>("host"));
            this.buyerOrderDb = new BuyerOrderService(dbs.GetCollection<BuyerOrder>("buyer"));;
            this.userService = new UserService(dbs.GetCollection<User>("user"));
        }
    }
}