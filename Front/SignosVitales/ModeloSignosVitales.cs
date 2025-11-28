using System;
using System.Collections.Generic;
using System.Text;

namespace Front.SignosVitales
{
    internal class ModeloSignosVitales
    {
      
           
                private int idSigno;
                private int ciPaciente;
                private DateTime fecha;
                private TimeSpan hora;
                private int ritmoCardiaco;
                private int presionArterial;
                private decimal temperatura;
                private int oxigenacion;

                public int IdSigno
                {
                    get { return idSigno; }
                    set { idSigno = value; }
                }

                public int CiPaciente
                {
                    get { return ciPaciente; }
                    set { ciPaciente = value; }
                }

                public DateTime Fecha
                {
                    get { return fecha; }
                    set { fecha = value; }
                }

                public TimeSpan Hora
                {
                    get { return hora; }
                    set { hora = value; }
                }

                public int RitmoCardiaco
                {
                    get { return ritmoCardiaco; }
                    set { ritmoCardiaco = value; }
                }

                public int PresionArterial
                {
                    get { return presionArterial; }
                    set { presionArterial = value; }
                }

                public decimal Temperatura
                {
                    get { return temperatura; }
                    set { temperatura = value; }
                }

                public int Oxigenacion
                {
                    get { return oxigenacion; }
                    set { oxigenacion = value; }
                }


               


                public ModeloSignosVitales(int pIdSigno, int pCiPaciente, DateTime pFecha, TimeSpan pHora,
                                          int pRitmoCardiaco, int pPresionArterial, decimal pTemperatura, int pOxigenacion)
                {
                    idSigno = pIdSigno;
                    ciPaciente = pCiPaciente;
                    fecha = pFecha;
                    hora = pHora;
                    ritmoCardiaco = pRitmoCardiaco;
                    presionArterial = pPresionArterial;
                    temperatura = pTemperatura;
                    oxigenacion = pOxigenacion;
                }
            
        
    }
}
