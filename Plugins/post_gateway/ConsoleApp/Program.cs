using System;
using System.Collections.Generic;
using FrotelServiceLibrary;
using FrotelServiceLibrary.Enum;
using FrotelServiceLibrary.Input;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            test();

            Console.ReadKey();
        }

        static async void test()
        {


            using (PostGatewayServiceManager froletServiceManager = new PostGatewayServiceManager())
            {
                var loginResult = await froletServiceManager.Login(new LoginInput
                {
                    UserName = "postbar",
                    Password = "2a1234@A!@#$"
                });

                var newshopoutput = await froletServiceManager.NewShop(new NewShopInput
                {
                    Site = "test.com",
                    ManagerName = "فرزاد",
                    Name = "فرزاد",
                    Email = "test @test.com",
                    Mobile = "0913",
                    Tel = "0313",
                    PostalCode = "818",
                    City = 81789,
                    State = 6,
                    Address = "ادرس cbv بلیب یب",
                    NationalCode = "129",
                    UserName = "farzad3",
                    FishDateYear = "1399",
                    FishDateMonth = "02",
                    FishDateDay = "13"
                }, loginResult.Cookies);
            }
        }
    }

}