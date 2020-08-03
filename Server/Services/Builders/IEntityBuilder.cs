using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services.Builders
{
    public interface IEntityBuilder<T,M> where T : class where M : IEntityModelView
    {
        public void ConfigureBuilder(ApplicationContext context);
        public Task<T> GetEntity(object id);
        public Task<M> GetModelView(object id);
        public Task<T> Create(M modelView);
    }
}
