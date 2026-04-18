using gsst.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace gsst.Model.FuelStuff
{
    public class Pump
    {
        public EventHandler StatusChanged;
        private PumpStatus _status = PumpStatus.Disabled;

        private string _name;


        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                if (string.IsNullOrEmpty(value)) throw new ArgumentException("Name cannot be empty");
                if (value.Length > 20) throw new ArgumentException("Name cannot be longer than 20 characters");
                _name = value;
            }
        }

        public PumpStatus Status 
        { 
            get => _status;
            set
            {
                if (_status == value) return;
                _status = value;
                StatusChanged?.Invoke(this, EventArgs.Empty);
            } 
        }

        public bool IsAvailable()
        {
            return Status == PumpStatus.Free;
        }

        public List<Tank> ConnectedTanks { get; set; } = new List<Tank>();
    }
}
