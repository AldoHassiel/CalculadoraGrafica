using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Proyecto1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /* AJUSTES GENERALES DE LA CONSOLA */
            Console.Title = "Calculadora";
            Console.WindowHeight = 12;
            Console.CursorVisible = false;

            /* VARIABLES NECESARIAS: */
            ConsoleKeyInfo teclaPulsada;
            string numeroTemporal = "";
            string historialDeOperaciones = "";
            char operadorActual;
            char operadorAnterior = '\0';
            string num1 = "", num2 = "", resultado = "";
            double n1 = 0, n2 = 0, res = 0;

            //Coordenadas de los numeros grandes:
            int coordNumX = Console.WindowWidth - 5;
            int coordNumY = 4;
            //Coordenadas de las operaciones:
            int coordOpX = Console.WindowWidth - 5;
            int coordOpY = 2;

            /* LOGICA PRINCIPAL*/
            MostarBordes();
            while (true)
            {
                if (numeroTemporal == "")
                {
                    BorrarNumeros(coordNumY);
                    DibujarNumeros("0", coordNumX, coordNumY);
                }
                //Registra la tecla pulsada
                teclaPulsada = Console.ReadKey(true);
                //Detecta si es un numero del 0-9 o si es es un '.'
                if (Char.IsNumber(teclaPulsada.KeyChar) || teclaPulsada.KeyChar == '.')
                {
                    //Comprueba que no existan varios puntos decimales en el numero
                    if (!(numeroTemporal.Contains(".") && teclaPulsada.KeyChar == '.'))
                    {
                        //Evita que el numero sea demaciado largo y se salga de la pantalla
                        if (((numeroTemporal.Replace(",", "").Length > 14 && !numeroTemporal.Contains("-") ||
                            numeroTemporal.Replace(",", "").Length > 15 && numeroTemporal.Contains("-"))) &&
                            teclaPulsada.KeyChar != '.' && !numeroTemporal.Contains("."))
                        {
                            continue;
                        }
                        //Agrega el el nuevo numero tecleado al almacenador
                        numeroTemporal += teclaPulsada.KeyChar;
                        //Le da un formato linde
                        DarFormatoAlNumero(ref numeroTemporal, "Numero");
                        //Lo muestra
                        BorrarNumeros(coordNumY);
                        DibujarNumeros(numeroTemporal, coordNumX, coordNumY);
                    }
                }
                //Detecta si es un operador o se trata de un numero negativo
                else if (teclaPulsada.KeyChar == '+' || teclaPulsada.KeyChar == '-' || teclaPulsada.KeyChar == '*' ||
                        teclaPulsada.KeyChar == '/')
                {
                    //Excepciones con el operador '-'
                    if (teclaPulsada.KeyChar == '-')
                    {
                        //Si se trata solo de un numero negativo (Falsa alarma, fak)
                        if ((numeroTemporal != "" && numeroTemporal[0] != '-' && !char.IsNumber(numeroTemporal[0]) ||
                                numeroTemporal == "") && operadorAnterior != '+')
                        {
                            numeroTemporal += teclaPulsada.KeyChar;
                            DarFormatoAlNumero(ref numeroTemporal, "Numero");
                            BorrarNumeros(coordNumY);
                            DibujarNumeros(numeroTemporal, coordNumX, coordNumY);
                        }
                        /* Cambia el signo si se quiere sumar un numero negativo:
                         *  555 + -2 -> 555 - 2
                         */
                        else if (teclaPulsada.KeyChar == '-' && operadorAnterior == '+')
                        {
                            operadorAnterior = '-';
                            historialDeOperaciones = $"{num1} {operadorAnterior}";
                            BorrarHistorial(coordOpY);
                            DibujarHistorial(historialDeOperaciones, coordOpX, coordOpY);
                        }
                        //Si se trataba de un operador jaja voy a llorar (codigo duplicado)
                        else if (!numeroTemporal.Contains("-") || (numeroTemporal.Contains("-") && numeroTemporal[0] == '-' && numeroTemporal.Length > 1))
                        {
                            operadorActual = teclaPulsada.KeyChar;
                            //Si el usuario anteriormente habia tecleado un numero, este se almacena ya sea en n1 o n2.
                            if (num1 == "" && (numeroTemporal != "" || numeroTemporal == "-"))
                            {
                                num1 = numeroTemporal;
                                n1 = Convert.ToDouble(num1);
                                DarFormatoAlNumero(ref num1, "Numero");
                                operadorAnterior = teclaPulsada.KeyChar;
                                historialDeOperaciones = $"{num1} {operadorAnterior}";
                            }
                            else if (num2 == "" && (numeroTemporal != "" || numeroTemporal == "-"))
                            {
                                num2 = numeroTemporal;
                                n2 = Convert.ToDouble(num2);
                                DarFormatoAlNumero(ref num2, "Numero");
                            }
                            //Realiza la operación automaticamente al tener n1, un numero ingresado y anteriormente un operador
                            if (num1 != "" && num2 != "" && numeroTemporal != "")
                            {
                                res = RealizarOperacion(n1, n2, operadorAnterior);
                                resultado = res.ToString();
                                num1 = resultado;
                                n1 = Convert.ToDouble(num1);
                                DarFormatoAlNumero(ref resultado, "Resultado");
                                historialDeOperaciones = $"{resultado} {operadorActual}";
                                BorrarHistorial(coordOpY);
                                BorrarNumeros(coordNumY);
                                DibujarNumeros(resultado, coordNumX, coordNumY);
                                num2 = "";
                                n2 = 0;
                                operadorAnterior = teclaPulsada.KeyChar;
                            }
                            BorrarHistorial(coordOpY);
                            DibujarHistorial(historialDeOperaciones, coordOpX, coordOpY);
                            numeroTemporal = "";
                        }
                    }
                    /* Cambia el signo si se quiere restar un numero negativo:
                         *  555 + -2 -> 555 - 2
                    */
                    else if (teclaPulsada.KeyChar == '+' && operadorAnterior == '-')
                    {
                        operadorAnterior = '+';
                        historialDeOperaciones = $"{num1} {operadorAnterior}";
                        BorrarHistorial(coordOpY);
                        DibujarHistorial(historialDeOperaciones, coordOpX, coordOpY);
                    }
                    /*Guarda los numeros tecleados, almacena el operador y espera otro numero,y 
                     * en ocaciones tambien resuelve traka*/
                    else
                    {
                        if (numeroTemporal != "" && numeroTemporal.Length == 1 && numeroTemporal[0] == '-') continue;
                        operadorActual = teclaPulsada.KeyChar;
                        //Si el usuario anteriormente habia tecleado un numero, este se almacena ya sea en n1 o n2.
                        if (num1 == "" && (numeroTemporal != "" || numeroTemporal == "-"))
                        {
                            num1 = numeroTemporal;
                            n1 = Convert.ToDouble(num1);
                            DarFormatoAlNumero(ref num1, "Numero");
                            operadorAnterior = teclaPulsada.KeyChar;
                            historialDeOperaciones = $"{num1} {operadorAnterior}";
                        }
                        else if (num2 == "" && (numeroTemporal != "" || numeroTemporal == "-"))
                        {
                            num2 = numeroTemporal;
                            n2 = Convert.ToDouble(num2);
                            DarFormatoAlNumero(ref num2, "Numero");
                        }
                        //Realiza la operación automaticamente al tener n1, un numero ingresado y anteriormente un operador
                        if (num1 != "" && num2 != "" && numeroTemporal != "")
                        {
                            res = RealizarOperacion(n1, n2, operadorAnterior);
                            resultado = res.ToString();
                            num1 = resultado;
                            n1 = Convert.ToDouble(num1);
                            DarFormatoAlNumero(ref resultado, "Resultado");
                            historialDeOperaciones = $"{resultado} {operadorActual}";
                            BorrarHistorial(coordOpY);
                            BorrarNumeros(coordNumY);
                            DibujarNumeros(resultado, coordNumX, coordNumY);
                            num2 = "";
                            n2 = 0;
                            operadorAnterior = teclaPulsada.KeyChar;
                        }
                        BorrarHistorial(coordOpY);
                        DibujarHistorial(historialDeOperaciones, coordOpX, coordOpY);
                        numeroTemporal = "";
                    }
                }
                //Detecta si se dió enter y se realiza la operacion
                else if (teclaPulsada.Key == ConsoleKey.Enter)
                {
                    if (num1 != "" && numeroTemporal != "")
                    {
                        num2 = numeroTemporal;
                        n2 = Convert.ToDouble(num2);
                        DarFormatoAlNumero(ref num2, "Numero");
                        res = RealizarOperacion(n1, n2, operadorAnterior);
                        resultado = res.ToString();
                        historialDeOperaciones = $"{num1} {operadorAnterior} {num2} =";
                        DarFormatoAlNumero(ref resultado, "Resultado");
                        BorrarHistorial(coordOpY);
                        BorrarNumeros(coordNumY);
                        DibujarNumeros(resultado, coordNumX, coordNumY);
                        num2 = "";
                        n2 = 0;
                        operadorAnterior = teclaPulsada.KeyChar;
                        DibujarHistorial(historialDeOperaciones, coordOpX, coordOpY);
                        numeroTemporal = res.ToString();
                        num1 = "";
                        n1 = 0;
                    }
                }
                //Detecta si quiere borrar un numero
                else if (teclaPulsada.Key == ConsoleKey.Backspace && numeroTemporal != "")
                {
                    numeroTemporal = numeroTemporal.Substring(0, numeroTemporal.Length - 1);
                    if (numeroTemporal.EndsWith(",")) numeroTemporal = numeroTemporal.Substring(0, numeroTemporal.Length - 1);
                    BorrarNumeros(coordNumY);
                    DibujarNumeros(numeroTemporal, coordNumX, coordNumY);
                }
                else if (teclaPulsada.Key == ConsoleKey.C && numeroTemporal != "")
                {
                    numeroTemporal = "";
                    BorrarNumeros(coordNumY);
                }
            }
        }
        public static void MostarBordes()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            //Bordes horizontales => 'X'
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("#");
                Console.SetCursorPosition(i, 10);
                Console.Write("#");
            }
            //Bordes verticales => 'Y'
            for (int i = 0; i < 10; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("#");
                Console.SetCursorPosition(Console.WindowWidth - 1, i);
                Console.Write("#");
            }
            Console.WriteLine();
            Console.ResetColor();
        }
        private static void DarFormatoAlNumero(ref string texto, string tipo)
        {
            double.TryParse(texto, out double n);
            //Formato para los numeros que vaya ingresando o los ya almacenados en el num temporal
            if (tipo == "Numero")
            {
                //Formato para cuando el primer 'numero' es un punto: '.' -> '0.'
                if (texto[0] == '.')
                {
                    texto = "0.";
                }
                else if (texto[0] == '0' && !texto.Contains(".") && texto.Length == 1) // '000' -> '0'
                {
                    texto = "0";
                }
                //Formato para cuando es 0 y otro numero: 04 -> 4
                else if (texto.Length > 1 && texto[0] == '0' && texto[1] != '.')
                {
                    texto = texto[1].ToString();
                }
                //Formato para cuando es -0 y otro numero: -05 -> -5
                else if (texto.Length > 2 && texto[0] == '-' && texto[1] == '0' && texto[2] != '.')
                {
                    texto = texto[0].ToString() + texto[2].ToString();
                }
                //Formato de miles y termina en .0: '1000.0' -> '1,000.'
                if ((n >= 1000 || n <= -1000) && n == Math.Floor(n) && texto.EndsWith("0") &&
                    texto.Contains("."))
                {
                    texto = n.ToString("0,0") + "." + texto.Split('.')[1];
                }
                else if ((n >= 1000 || n <= -1000) && n == Math.Floor(n) && texto.EndsWith("."))
                {
                    texto = n.ToString("0,0") + ".";
                }
                else if ((n >= 1000 || n <= -1000) && n == Math.Floor(n) && texto.EndsWith("0") &&
                    texto[texto.Length - 2] == '.')
                {
                    texto = n.ToString("0,0") + "." + texto.Split('.')[1];
                }
                //Formato de miles: '1000' -> '1,000'
                else if ((n >= 1000 || n <= -1000) && n == Math.Floor(n) &&
                    !texto.Contains("."))
                {
                    texto = n.ToString("0,0");
                }
                //Formato de numeros negativos
                if (texto.Contains("-") && texto.Length > 1)
                {
                    if (texto[1] == '.')
                    {
                        texto = "-0.";
                    }
                    if (texto.Length > 2 && texto[1] == '0' && texto[2] == '0')
                    {
                        texto = "-0";
                    }
                    if (texto[1] == '-')
                    {
                        texto = "-";
                    }
                }
                //Formato para solo admitir 9 numeros decimales
                if (texto.Contains(".") && texto.Split('.')[1].Length > 8)
                {
                    texto = texto.Split('.')[0] + "." + texto.Split('.')[1].Substring(0, texto.Split('.')[1].Length - 1);
                }
            }
            //Formato para los resultados de las operaciones (A veces se redondea)
            else if (tipo == "Resultado")
            {
                //Formato de miles con punto decimal y despues un cero: '1000.0' -> '1,000'
                if (!texto.Contains("E"))
                {
                    if ((n >= 1000 || n <= -1000) && n == Math.Floor(n))
                    {
                        texto = n.ToString("0,0");
                    }
                    //Formato de miles con punto decimal: '1000.4526' -> '1,000.452'
                    else if ((n >= 1000 || n <= -1000) && n != Math.Floor(n))
                    {
                        texto = n.ToString("0,0.000");
                    }
                }
                else
                {
                    texto = n.ToString("0.####E+0");
                }
            }
        }
        public static char[,] ObtenerMatrizDelNumero(char n)
        {
            char[,] numero = new char[5, 3];
            switch (n)
            {
                case '0':
                    char[,] cero = {
                        {'█', '█', '█' },
                        {'█', ' ', '█' },
                        {'█', ' ', '█'},
                        {'█', ' ', '█' },
                        {'█', '█', '█' }
                    };
                    numero = cero;
                    break;
                case '1':
                    char[,] uno = {
                        {' ', '█', ' '},
                        {'█', '█', ' '},
                        {' ', '█', ' '},
                        {' ', '█', ' '},
                        {'█', '█', '█'}
                    };
                    numero = uno;
                    break;
                case '2':
                    char[,] dos = {
                        {'█', '█', '█'},
                        {' ', ' ', '█'},
                        {'█', '█', '█'},
                        {'█', ' ', ' '},
                        {'█', '█', '█'}
                    };
                    numero = dos;
                    break;
                case '3':
                    char[,] tres = {
                            {'█', '█', '█'},
                            {' ', ' ', '█'},
                            {'█', '█', '█'},
                            {' ', ' ', '█'},
                            {'█', '█', '█'}
                        };
                    numero = tres;
                    break;
                case '4':
                    char[,] cuatro = {
                            {'█', ' ', '█'},
                            {'█', ' ', '█'},
                            {'█', '█', '█'},
                            {' ', ' ', '█'},
                            {' ', ' ', '█'}
                        };
                    numero = cuatro;
                    break;
                case '5':
                    char[,] cinco = {
                            {'█', '█', '█'},
                            {'█', ' ', ' '},
                            {'█', '█', '█'},
                            {' ', ' ', '█'},
                            {'█', '█', '█'}
                        };
                    numero = cinco;
                    break;
                case '6':
                    char[,] seis = {
                            {'█', '█', '█'},
                            {'█', ' ', ' '},
                            {'█', '█', '█'},
                            {'█', ' ', '█'},
                            {'█', '█', '█'}
                        };
                    numero = seis;
                    break;
                case '7':
                    char[,] siete = {
                            {'█', '█', '█'},
                            {' ', ' ', '█'},
                            {' ', ' ', '█'},
                            {' ', ' ', '█'},
                            {' ', ' ', '█'}
                        };
                    numero = siete;
                    break;
                case '8':
                    char[,] ocho = {
                            {'█', '█', '█'},
                            {'█', ' ', '█'},
                            {'█', '█', '█'},
                            {'█', ' ', '█'},
                            {'█', '█', '█'}
                        };
                    numero = ocho;
                    break;
                case '9':
                    char[,] nueve = {
                            {'█', '█', '█'},
                            {'█', ' ', '█'},
                            {'█', '█', '█'},
                            {' ', ' ', '█'},
                            {' ', ' ', '█'}
                        };
                    numero = nueve;
                    break;
                case '.':
                    char[,] punto = {
                            {' ', ' ', ' '},
                            {' ', ' ', ' '},
                            {' ', ' ', ' '},
                            {' ', ' ', ' '},
                            {'█', ' ', ' '}
                        };
                    numero = punto;
                    break;
                case ',':
                    char[,] coma = {
                            {' ', ' ', ' '},
                            {' ', ' ', ' '},
                            {' ', ' ', ' '},
                            {' ', '█', ' '},
                            {'█', '█', ' '}
                        };
                    numero = coma;
                    break;
                case '-':
                    char[,] menos = {
                            {' ', ' ', ' '},
                            {' ', ' ', ' '},
                            {'█', '█', '█'},
                            {' ', ' ', ' '},
                            {' ', ' ', ' '}
                        };
                    numero = menos;
                    break;
                case 'E':
                    char[,] e = {
                            {'█', '█', '█'},
                            {'█', ' ', ' '},
                            {'█', '█', ' '},
                            {'█', ' ', ' '},
                            {'█', '█', '█'}
                        };
                    numero = e;
                    break;
                case '+':
                    char[,] mas = {
                            {' ', ' ', ' '},
                            {' ', '█', ' '},
                            {'█', '█', '█'},
                            {' ', '█', ' '},
                            {' ', ' ', ' '}
                        };
                    numero = mas;
                    break;
            }
            return numero;
        }
        public static void DibujarCaracter(char n, int x, int y)
        {
            char[,] caracter = ObtenerMatrizDelNumero(n);
            if (n == 'E') Console.ForegroundColor = ConsoleColor.Red;
            if (n == '+') Console.ForegroundColor = ConsoleColor.Green;
            int xCopia = x;
            for (int i = 0; i < caracter.GetLength(0); i++)
            {
                for (int j = 0; j < caracter.GetLength(1); j++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(caracter[i, j]);
                    x++;
                }
                y++;
                x = xCopia;
            }
            if (n == 'E' || n == '+') Console.ResetColor();
        }
        public static void DibujarNumeros(string numeros, int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.White;
            //Establece la coordenada x según la longitud del numero
            foreach (char cosa in numeros)
            {
                if (int.TryParse(cosa.ToString(), out int num) || cosa == '-' || cosa == 'E' || cosa == '+')
                {
                    x -= 4;
                }
                else if (cosa == '.')
                {
                    x -= 2;
                }
                else if (cosa == ',')
                {
                    x -= 3;
                }
            }
            //Recorre todos los numeros
            foreach (char n in numeros)
            {
                //Dibujar el numero actual
                DibujarCaracter(n, x, y);
                //Deja espacio entre cada numero, punto o coma
                if (n == '.')
                {
                    x += 2;
                }
                else if (n == ',')
                {
                    x += 3;
                }
                else
                {
                    x += 4;
                }
            }
            Console.ResetColor();
        }
        public static void DibujarHistorial(string historial, int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            x -= historial.Length + 1;
            Console.SetCursorPosition(x, y);
            Console.Write(historial);
            Console.ResetColor();
        }
        public static void BorrarNumeros(int y)
        {
            string longitud = "";
            string borrador = longitud.PadRight(Console.WindowWidth - 3, ' ');
            Console.SetCursorPosition(1, y);
            Console.Write(borrador);
            Console.SetCursorPosition(1, y + 1);
            Console.Write(borrador);
            Console.SetCursorPosition(1, y + 2);
            Console.Write(borrador);
            Console.SetCursorPosition(1, y + 3);
            Console.Write(borrador);
            Console.SetCursorPosition(1, y + 4);
            Console.Write(borrador);

        }
        public static void BorrarHistorial(int y)
        {
            string longitud = "";
            string borrador = longitud.PadRight(Console.WindowWidth - 3, ' ');
            Console.SetCursorPosition(1, y);
            Console.Write(borrador);
        }
        public static double RealizarOperacion(double n1, double n2, char operador)
        {
            switch (operador)
            {
                case '+':
                    return n1 + n2;
                case '-':
                    return n1 - n2;
                case '*':
                    return n1 * n2;
                case '/':
                    return n1 / n2;
            }
            return 0;
        }
    }
}
