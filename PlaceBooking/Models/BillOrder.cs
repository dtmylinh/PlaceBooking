namespace PlaceBooking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BillOrder
    {
        [Key]
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public decimal Total { get; set; }
        public DateTime CreateDate { get; set; }
        public int? PaymentType { get; set; }
        public int? PaymentStatus { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }
        public int UserId { get; set; }
    }
}
