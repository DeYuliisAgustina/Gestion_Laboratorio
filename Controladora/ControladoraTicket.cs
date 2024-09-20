﻿using Entidades;
using Microsoft.EntityFrameworkCore;
using Modelo;
using System.Collections.ObjectModel;

namespace Controladora
{
    public class ControladoraTicket
    {
        Context context;

        private ControladoraTicket()
        {
            context = new Context();
        }

        private static ControladoraTicket instancia;

        public static ControladoraTicket Instancia
        {

            get
            {
                if (instancia == null)
                    instancia = new ControladoraTicket();
                return instancia;
            }
        }
        public ReadOnlyCollection<Ticket> RecuperarTicket()
        {
            try
            {
                Context.Instancia.Tickets.ToList().AsReadOnly();
                return Context.Instancia.Tickets.ToList().AsReadOnly();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ReadOnlyCollection<Sede> RecuperarSedes()
        {
            try
            {
                Context.Instancia.Sedes.ToList().AsReadOnly();
                return Context.Instancia.Sedes.ToList().AsReadOnly();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ReadOnlyCollection<Laboratorio> RecuperarLaboratorios()
        {
            try
            {
                Context.Instancia.Laboratorios.ToList().AsReadOnly();
                return Context.Instancia.Laboratorios.ToList().AsReadOnly();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ReadOnlyCollection<Computadora> RecuperarComputadoras()
        {
            try
            {
                Context.Instancia.Computadoras.ToList().AsReadOnly();
                return Context.Instancia.Computadoras.ToList().AsReadOnly();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ReadOnlyCollection<Tecnico> RecuperarTecnicos()
        {
            try
            {
                Context.Instancia.Tecnicos.ToList().AsReadOnly();
                return Context.Instancia.Tecnicos.ToList().AsReadOnly();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string AgregarTicket(Ticket ticket)
        {
            try
            {
                var listaTickets = Context.Instancia.Tickets.ToList().AsReadOnly();
                var ticketEncontrado = Context.Instancia.Tickets.FirstOrDefault(t => t.TicketId == ticket.TicketId); // Verifico si el ticket ya existe 
                if (ticketEncontrado == null)
                {
                    var ticketExistente = listaTickets.FirstOrDefault(t => t.Computadora.ComputadoraId == ticket.Computadora.ComputadoraId); // Verifico si ya existe un ticket con el código de computadora
                    if (ticketExistente == null)
                    {
                        Context.Instancia.Tickets.Add(ticket);

                        int agregados = Context.Instancia.SaveChanges();
                        if (agregados > 0)
                        {
                            return $"El ticket se agregó correctamente";
                        }
                        else return $"El ticket no se ha podido agregar";
                    }
                    else
                    {
                        return $"Ya existe un ticket con el código de computadora {ticket.Computadora.CodigoComputadora}";
                    }
                }
                else
                {
                    return $"El ticket ya existe";
                }
            }
            catch (Exception ex)
            {
                return "Error desconocido" + ex;
            }
        }

        public string ModificarTicket(Ticket ticket)
        {
            try
            {
                var listaTickets = Context.Instancia.Tickets.ToList().AsReadOnly();
                var ticketEncontrado = listaTickets.FirstOrDefault(t => t.Computadora.CodigoComputadora == ticket.Computadora.CodigoComputadora); // Verifico si el ticket ya existe 
                if (ticketEncontrado != null)
                {
                    Context.Instancia.Tickets.Update(ticket);

                    int insertados = Context.Instancia.SaveChanges();


                    if (insertados > 0)
                    {
                        return $"El ticket se modificó correctamente";
                    }
                    else return $"El ticket no se ha podido modificar";
                }
                else
                {
                    return $"El ticket no existe";
                }
            }
            catch (Exception ex)
            {
                return "Error desconocido" + ex;
            }
        }

        public string EliminarTicket(Ticket ticket)
        {
            try
            {
                var ticketEncontrado = Context.Instancia.Tickets.FirstOrDefault(t => t.TicketId == ticket.TicketId);
                if (ticketEncontrado != null)
                {
                    Context.Instancia.Tickets.Remove(ticketEncontrado);
                    int eliminados = Context.Instancia.SaveChanges();
                    if (eliminados > 0)
                    {
                        return $"El ticket se eliminó correctamente";
                    }
                    else return $"El ticket no se ha podido eliminar";
                }
                else
                {
                    return $"El ticket no existe";
                }
            }
            catch (Exception ex)
            {
                return "Error desconocido" + ex;
            }
        }

        public int ContarTicketsPorTecnico(Tecnico tecnico)
        {
            try
            {
                var listaTickets = Context.Instancia.Tickets.ToList().AsReadOnly(); // Recupero todos los tickets 
                var ticketsPorTecnico = listaTickets.Where(t => t.Tecnico.TecnicoId == tecnico.TecnicoId).ToList(); // Filtro los tickets por tecnico para contar los tickets que tiene asignados cada tecnico
                return ticketsPorTecnico.Count; // Devuelvo la cantidad de tickets que tiene asignados el tecnico 
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
