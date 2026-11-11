using System;
using System.Globalization;
using System.Text;

namespace LaboratorioTema7
{
    internal class Program
    {
        static string[] codigos = { "AUTO-201", "AUTO-202", "AUTO-203", "AUTO-204", "AUTO-205", "AUTO-206" };
        static string[] modelos = { "SUV Interceptor", "Sedan Executive", "PickUp Titan", "City Hatchback", "Electric EV", "Van Carga" };
        static double[,] datos ={ { 28000.0, 2 }, { 18500.0, 4 }, { 35000.0, 1 }, { 14000.0, 5 }, { 42000.0, 1 }, { 22500.0, 3 } };
        static readonly CultureInfo culturaUsd = CultureInfo.GetCultureInfo("en-US");
        static bool ValidarDatos(double precio, int cantidad)
        {
            if (double.IsFinite(precio) && precio > 0 && cantidad > 0)
            {
                return true;
            }

            Console.WriteLine("Error: el precio y la cantidad deben ser mayores que 0.");
            return false;
        }

        static bool ValidarEntrada(bool mostrarDetalle)
        {
            bool todoValido = true;

            if (mostrarDetalle)
            {
                Console.WriteLine("******* MÓDULO VALIDAR ENTRADA *******");
            }

            for (int i = 0; i < codigos.Length; i++)
            {
                double cantidad = datos[i, 1];

                if (mostrarDetalle)
                {
                    Console.Write($"Validación de {codigos[i]}: ");
                }

                if (cantidad % 1 != 0 || cantidad > int.MaxValue || cantidad < int.MinValue)
                {
                    Console.WriteLine($"Error en {codigos[i]}: la cantidad debe ser un entero válido.");
                    todoValido = false;
                }
                else if (!ValidarDatos(datos[i, 0], (int)cantidad))
                {
                    todoValido = false;
                }
                else if (mostrarDetalle)
                {
                    Console.WriteLine("Datos válidos.");
                }
            }

            if (mostrarDetalle && todoValido)
            {
                Console.WriteLine("Todos los registros son válidos.");
            }

            return todoValido;
        }

        static void ImprimirMatrizDatos(double[,] matriz, string[] codigos)
        {
            Console.WriteLine("--- MATRIZ BIDIMENSIONAL DE DATOS (PRECIO Y CANTIDAD) ---");
            Console.WriteLine("Fila | Código   | Col 0 (Precio $) | Col 1 (Cantidad)");
            Console.WriteLine("------------------------------------------------------");

            for (int i = 0; i < matriz.GetLength(0); i++)
            {
                string precio = matriz[i, 0].ToString("N2", culturaUsd);
                Console.WriteLine($"[{i}]  | {codigos[i]} | $ {precio,14} | {matriz[i, 1],16:0}");
            }
        }

        static void ReporteYAcumuladores(double[] montosNetos)
        {
            double totalFinal = 0;
            int contadorPremium = 0;

            for (int i = 0; i < montosNetos.Length; i++)
            {
                totalFinal += montosNetos[i];

                if (montosNetos[i] > 50000.0)
                {
                    contadorPremium++;
                }
            }

            Console.WriteLine("MÉTRICAS RESUMEN:");
            Console.WriteLine($"  * Gran Total Acumulado    : $ {totalFinal.ToString("N2", culturaUsd)}");
            Console.WriteLine($"  * Ventas Premium (>$50,000): {contadorPremium} transacciones");
        }

        static void OrdenamientoBurbujaDescendente(
            double[,] matriz, string[] codigos, string[] modelos, double[] montosNetos)
        {
            int n = codigos.Length;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1 - i; j++)
                {
                    if (montosNetos[j] < montosNetos[j + 1])
                    {
                        double tempPrecio = matriz[j, 0];
                        matriz[j, 0] = matriz[j + 1, 0];
                        matriz[j + 1, 0] = tempPrecio;

                        double tempCantidad = matriz[j, 1];
                        matriz[j, 1] = matriz[j + 1, 1];
                        matriz[j + 1, 1] = tempCantidad;

                        string tempCodigo = codigos[j];
                        codigos[j] = codigos[j + 1];
                        codigos[j + 1] = tempCodigo;

                        string tempModelo = modelos[j];
                        modelos[j] = modelos[j + 1];
                        modelos[j + 1] = tempModelo;

                        double tempMonto = montosNetos[j];
                        montosNetos[j] = montosNetos[j + 1];
                        montosNetos[j + 1] = tempMonto;
                    }
                }
            }
        }

        static void MostrarReporteOrdenado(double[] montosNetos, bool reporteCompleto)
        {
            // Se ordenan copias para conservar el orden original de la matriz.
            double[,] matrizOrdenada = (double[,])datos.Clone();
            string[] codigosOrdenados = (string[])codigos.Clone();
            string[] modelosOrdenados = (string[])modelos.Clone();
            double[] montosOrdenados = (double[])montosNetos.Clone();

            OrdenamientoBurbujaDescendente(
                matrizOrdenada, codigosOrdenados, modelosOrdenados, montosOrdenados);

            Console.WriteLine("--- REPORTE ORDENADO POR MONTO NETO (DECRECIENTE) ---");
            for (int i = 0; i < codigosOrdenados.Length; i++)
            {
                string monto = montosOrdenados[i].ToString("N2", culturaUsd);
                Console.WriteLine(
                    $"{i + 1}. Código: {codigosOrdenados[i]} | Modelo: {modelosOrdenados[i],-15} | Monto: $ {monto}");
            }

            if (reporteCompleto)
            {
                Console.WriteLine();
                ReporteYAcumuladores(montosOrdenados);
                Console.WriteLine();
                Console.WriteLine("PRUEBA DE BÚSQUEDA SECUENCIAL:");
                BuscarCodigo(codigosOrdenados, modelosOrdenados, montosOrdenados, "AUTO-203");
            }
        }

        static void BuscarCodigo(
            string[] codigos, string[] modelos, double[] montos, string codigoBuscado)
        {
            if (string.IsNullOrWhiteSpace(codigoBuscado))
            {
                Console.WriteLine("Error: el código de búsqueda está vacío.");
                return;
            }

            codigoBuscado = codigoBuscado.Trim();

            for (int i = 0; i < codigos.Length; i++)
            {
                if (string.Equals(codigos[i], codigoBuscado, StringComparison.OrdinalIgnoreCase))
                {
                    string monto = montos[i].ToString("N2", culturaUsd);
                    Console.WriteLine(
                        $"  * Buscando '{codigoBuscado}': ¡ENCONTRADO! Modelo: {modelos[i]} | Monto: ${monto}");
                    return;
                }
            }

            Console.WriteLine($"Error: el código '{codigoBuscado}' no está registrado.");
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            double[] montosNetos = new double[codigos.Length];

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("======= CONTROL DE VENTAS AUTOMOTRICES (EVALUACIÓN 45 MIN) =======");
                Console.WriteLine("[1] Validar datos de entrada");
                Console.WriteLine("[2] Ver reporte completo y acumuladores");
                Console.WriteLine("[3] Ver ordenamiento burbuja descendente");
                Console.WriteLine("[4] Buscar vehículo por código");
                Console.WriteLine("[5] Salir del programa");
                Console.WriteLine("Ingrese opción:");

                string? entrada = Console.ReadLine();
                if (entrada == null)
                {
                    return;
                }

                if (!int.TryParse(entrada, out int opcion))
                {
                    Console.WriteLine("Opción no válida.");
                    continue;
                }

                if (opcion >= 2 && opcion <= 4)
                {
                    if (!ValidarEntrada(false))
                    {
                        continue;
                    }

                    for (int i = 0; i < codigos.Length; i++)
                    {
                        montosNetos[i] = datos[i, 0] * datos[i, 1];
                    }
                }

                switch (opcion)
                {
                    case 1:
                        ValidarEntrada(true);
                        break;
                    case 2:
                        ImprimirMatrizDatos(datos, codigos);
                        Console.WriteLine();
                        MostrarReporteOrdenado(montosNetos, true);
                        break;
                    case 3:
                        MostrarReporteOrdenado(montosNetos, false);
                        break;
                    case 4:
                        Console.WriteLine("Ingrese un código (ej. AUTO-201):");
                        BuscarCodigo(codigos, modelos, montosNetos, Console.ReadLine() ?? "");
                        break;
                    case 5:
                        Console.WriteLine("Saliendo del programa.");
                        return;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }
    }
}
