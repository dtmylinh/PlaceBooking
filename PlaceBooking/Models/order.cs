using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace PlaceBooking.Models
{
    [Table("order")]
    public partial class Order
    {
        [Key]
        public int Id { get; set; }
        public int GuestTotal { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Gioitinh { get; set; }
        public string Quoctich { get; set; }
        public string Mess { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string DeliveryPaymentMethod { get; set; }
        public int StatusPayment { get; set; }
        public double Total { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public string NameRoom { get; set; }
        public string ServiceId { get; set; }
        public string MenuId { get; set; }
        public DateTime DepartureDate { get; set; }
        public string Scope { get; set; }
    }
}
