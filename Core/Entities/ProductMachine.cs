namespace Core.Entities
{
    public class ProductMachine : BaseAuditableEntity
    {
        public bool IsNew { get; set; }
        public Product Product { get; set; }
    }
}