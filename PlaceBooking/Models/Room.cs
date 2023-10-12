namespace PlaceBooking.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("room")]
    public partial class Room
    {
       
         [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PlaceDetail { get; set; }
        public string RoomType { get; set; }
        public string Img { get; set; }      
        public string DepartureAddress { get; set; }
        public string FullAddress { get; set; }
        public int? GuestTotal { get; set; }
        public double Price { get; set; }
        public double PriceSale { get; set; }
        public int? Promote { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public int Status { get; set; }
        public int IsBooking { get; set; }
    }
}
