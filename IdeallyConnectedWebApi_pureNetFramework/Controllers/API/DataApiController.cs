﻿using System;
using System.Linq;
using System.Net;
using System.Web.Http; // RoutePrefixAttribute(), ApiController
using System.Net.Http;
using IdeallyConnectedWebApi_pureNetFramework.Models;

namespace IdeallyConnectedWebApi_pureNetFramework.API
{
    [RoutePrefix("api")]
    public class DataApiController: ApiController
    {
    /*
        [HttpGet]
        [Route("users")]
        public HttpResponseMessage GetUsers(HttpRequestMessage request)
        {
            var users = DataFactory.GetUsers();
            return request.CreateResponse<UserModel[]>(HttpStatusCode.OK, users.ToArray());
        }
        
        [HttpGet]
        [Route("user/{userId}")]
        public HttpResponseMessage GetUser(HttpRequestMessage request, int userId)
        {
            var users = DataFactory.GetUsers();
            var user = users.FirstOrDefault(item => item.UserId == userId);

            return request.CreateResponse<UserModel>(HttpStatusCode.OK, user);
        }
        */
    }

    
}