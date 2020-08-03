using Server.Services.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.ModelViews;

namespace Server.Services
{
    public interface IEntityService<T,M> where T : class where M : IEntityModelView
    {
        public Task<M> Get(object Id, IEntityBuilder<T, M> builder);
        public Task<M> Create(M modelView, IEntityBuilder<T, M> builder);
        public Task<M> Update(M modelView);
        public Task<bool> Delete(object id, string userLogin);
    }
}
