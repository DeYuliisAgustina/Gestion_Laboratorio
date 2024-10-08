﻿using Entidades;
using Microsoft.EntityFrameworkCore;
using Modelo;
using System.Collections.ObjectModel;

namespace Controladora
{
    public class ControladoraComputadora
    {
        Context context;

        private ControladoraComputadora()
        {
            context = new Context();
        }

        private static ControladoraComputadora instancia;

        public static ControladoraComputadora Instancia
        {

            get
            {
                if (instancia == null)
                    instancia = new ControladoraComputadora();
                return instancia;
            }
        }

        public ReadOnlyCollection<Computadora> RecuperarComputadoras()
        {
            try
            {
                Context.Instancia.Computadoras.ToList().AsReadOnly();
                return Context.Instancia.Computadoras.ToList().AsReadOnly();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReadOnlyCollection<Laboratorio> RecuperarLaboratorios()
        {
            try
            {
                Context.Instancia.Laboratorios.ToList().AsReadOnly();
                return Context.Instancia.Laboratorios.ToList().AsReadOnly();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ReadOnlyCollection<Sede> RecuperarSedes()
        {
            try
            {
                Context.Instancia.Sedes.ToList().AsReadOnly();
                return Context.Instancia.Sedes.ToList().AsReadOnly();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string AgregarComputadora(Computadora computadora)
        {
            try
            {
                var listaLaboratorios = Context.Instancia.Laboratorios.ToList().AsReadOnly();
                var laboratorioEncontrado = listaLaboratorios.FirstOrDefault(l => l.LaboratorioId == computadora.LaboratorioId); //busco el laboratorio por id para verificar que exista
                if (laboratorioEncontrado != null)
                {
                    if (laboratorioEncontrado.Computadoras.Count < laboratorioEncontrado.CapacidadMaxima) //verifico que la capacidad del laboratorio no se haya superado para agregar la computadora
                    {
                        var listaComputadoras = Context.Instancia.Computadoras.ToList().AsReadOnly();
                        var computadoraEncontrada = listaComputadoras.FirstOrDefault(c => c.CodigoComputadora.ToLower() == computadora.CodigoComputadora.ToLower() && c.LaboratorioId == computadora.LaboratorioId); //busco la computadora por codigo y laboratorio para verificar que no se repita
                        if (computadoraEncontrada == null)
                        {
                            Context.Instancia.Computadoras.Add(computadora);
                            int insertados = Context.Instancia.SaveChanges();
                            if (insertados > 0)
                            {
                                return $"La computadora se agregó correctamente";
                            }
                            else return $"La computadora no se ha podido agregar";
                        }
                        else
                        {
                            return $"La computadora ya existe";
                        }
                    }
                    else
                    {
                        return $"Capacidad superada, la capacidad maxima es de {laboratorioEncontrado.CapacidadMaxima} computadoras";
                    }
                }
                else
                {
                    return $"El laboratorio no existe";
                }
            }
            catch (Exception ex)
            {
                return "Error desconocido" + ex;
            }
        }

        public string ModificarComputadora(Computadora computadora)
        {
            try
            {
                var listaComputadoras = Context.Instancia.Computadoras.ToList().AsReadOnly();
                var computadoraEncontrada = listaComputadoras.FirstOrDefault(c => c.CodigoComputadora.ToLower() == computadora.CodigoComputadora.ToLower() && c.LaboratorioId == computadora.LaboratorioId); //busco la computadora por codigo y laboratorio para verificar que exista
                if (computadoraEncontrada != null)
                {
                    Context.Instancia.Computadoras.Update(computadora);
                    int insertados = Context.Instancia.SaveChanges();
                    if (insertados > 0)
                    {
                        return $"La computadora se modificó correctamente";
                    }
                    else return $"La computadora no se ha podido modificar";
                }
                else
                {
                    return $"La computadora no existe";
                }
            }
            catch (Exception ex)
            {
                return "Error desconocido" + ex;
            }
        }

        public string EliminarComputadora(Computadora computadora)
        {
            try
            {
                var listaComputadoras = Context.Instancia.Computadoras.ToList().AsReadOnly();
                var computadoraEncontrada = listaComputadoras.FirstOrDefault(c => c.CodigoComputadora.ToLower() == computadora.CodigoComputadora.ToLower() && c.LaboratorioId == computadora.LaboratorioId); //busco la computadora por codigo y laboratorio para verificar que exista
                if (computadoraEncontrada != null) 
                {
                    Context.Instancia.Computadoras.Remove(computadora);
                    int insertados = Context.Instancia.SaveChanges();
                    if (insertados > 0)
                    {
                        return $"La computadora se eliminó correctamente";
                    }
                    else return $"La computadora no se ha podido eliminar";
                }
                else
                {
                    return $"La computadora no existe";
                }
            }
            catch (Exception)
            {
                return "Error desconocido";
            }
        }
    }
}
