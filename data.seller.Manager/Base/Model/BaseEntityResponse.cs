using System.Collections.Generic;

namespace data.seller.Manager.Base.Model
{
    public class BaseEntityResponse<TEntity> : BaseResponse
    {
        public BaseEntityResponse(bool status, IList<DetailError> detailErrors) : base(status, detailErrors)
        {
        }

        public BaseEntityResponse()
        {
        }

        public TEntity Data { get; set; }

        public virtual BaseEntityResponse<TEntity> SetData(TEntity data)
        {
            Data = data;
            Status = true;
            return this;
        }
    }
}
