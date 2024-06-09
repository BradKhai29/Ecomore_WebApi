using DataAccess.DataSeedings;
using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class OrderStatusEntity : GuidEntity
    {
        public string Name { get; set; }

        public static OrderStatusEntity GetNextStatus(Guid currentStatusId)
        {
            var statusList = OrderStatuses.GetValues().ToList();
            for (int i = 0; i < statusList.Count; i++)
            {
                var status = statusList[i];
                if (status.Id == currentStatusId)
                {
                    return statusList[i + 1];
                }
            }

            return OrderStatuses.GetStatusById(currentStatusId);
        }

        #region Relationships
        public IEnumerable<OrderEntity> Orders { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "OrderStatuses";
        }
        #endregion
    }
}
