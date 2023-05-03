using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiberController
{

    [FirestoreData]
    public class Registro
    {
        [FirestoreProperty]
        public int ID { get; set; }
        [FirestoreProperty]
        public String NombrePC { get; set; }
        [FirestoreProperty]
        public int FechaInicial { get; set; }
        [FirestoreProperty]
        public int FechaFinal { get; set; }
        [FirestoreProperty]
        public int ImpresionesNegro { get; set; }
        [FirestoreProperty]
        public int Escaneos { get; set; }
        [FirestoreProperty]
        public double CostoAdicional { get; set; }
        [FirestoreProperty]
        public int TiempoTotal { get; set; }
        [FirestoreProperty]
        public String TotalPagar { get; set; }
        [FirestoreProperty]
        public int Estatus { get; set; }

        [FirestoreProperty]
        public int PcEstado { get; set; }

    }

}
