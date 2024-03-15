using System;
using System.Threading;

namespace Calculadora
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
            string numeroTemporal = "0";
            string historialDeOperaciones = "";
            char operadorAnterior = '\0';
            string num1 = "", num2 = "", resultado = "";
            double n1 = 0, n2 = 0, res = 0;
            bool operacionTermianda = false;
            //Coordenadas de los numeros grandes:
            int coordNumX = Console.WindowWidth - 5;
            int coordNumY = 4;
            //Coordenadas de las operaciones:
            int coordOpX = Console.WindowWidth - 5;
            int coordOpY = 2;
            //Nueva logica
            MostarBordes();
            DibujarNumeros(numeroTemporal, coordNumX, coordNumY);
            while (true)
            {
                //Registrar la tecla pulsada
                teclaPulsada = Console.ReadKey(true);
                //Valida que es un numero del 0-9 o un punto
                if (Char.IsNumber(teclaPulsada.KeyChar) || teclaPulsada.KeyChar == '.')
                {
                    //Solo permite un punto decimal
                    if (numeroTemporal.Contains(".") && teclaPulsada.KeyChar == '.') continue;
                    //Evita que el numero sea demaciado largo y se salga de la pantalla
                    if (((numeroTemporal.Replace(",", "").Length > 14 && !numeroTemporal.Contains("-") ||
                        numeroTemporal.Replace(",", "").Length > 15 && numeroTemporal.Contains("-"))) &&
                        teclaPulsada.KeyChar != '.' && !numeroTemporal.Contains("."))
                    {
                        continue;
                    }
                    if (operacionTermianda)
                    {
                        ReiniciarTodo(ref numeroTemporal, ref num1, ref num2, ref n1, ref n2, ref operadorAnterior,
                                    ref historialDeOperaciones, ref operacionTermianda, coordOpY, coordNumX, coordNumY);
                        operacionTermianda = false;
                    }
                    //Agrega el el nuevo numero tecleado al almacenador
                    numeroTemporal += teclaPulsada.KeyChar;
                    //Le da un formato linde
                    DarFormatoAlNumero(ref numeroTemporal, "Numero");
                    //Lo muestra
                    BorrarNumeros(coordNumY);
                    DibujarNumeros(numeroTemporal, coordNumX, coordNumY);
                }
                //Valida si es un operador
                else if (numeroTemporal != "-" && (teclaPulsada.KeyChar == '+' || teclaPulsada.KeyChar == '-' ||
                    teclaPulsada.KeyChar == '*' || teclaPulsada.KeyChar == '/'))
                {
                    //No se, pero funciona, asi que NO QUITAR LA LINEA.
                    operacionTermianda = false;
                    //Quizas solo se trata de un numero negativo
                    if ((teclaPulsada.KeyChar == '-' && numeroTemporal == "") ||
                        (teclaPulsada.KeyChar == '-' && numeroTemporal == "0"))
                    {
                        numeroTemporal = teclaPulsada.KeyChar.ToString();
                        BorrarNumeros(coordNumY);
                        DibujarNumeros(numeroTemporal, coordNumX, coordNumY);
                    }
                    //Pues si es un operador 
                    else
                    {
                        //Almacena el numero verificando si se trata del n1 o del n2
                        if (num1 == "" && numeroTemporal != "" && numeroTemporal != "-")
                        {
                            num1 = numeroTemporal;
                            n1 = Convert.ToDouble(num1);
                            operadorAnterior = teclaPulsada.KeyChar;
                            historialDeOperaciones = $"{num1} {operadorAnterior}";
                        }
                        else if (num2 == "" && numeroTemporal != "" && numeroTemporal != "-")
                        {
                            num2 = numeroTemporal;
                            n2 = Convert.ToDouble(num2);
                        }
                        //Comprueba si ya hay dos numeros para asi ya proceder con la operacion
                        if (num1 != "" && num2 != "")
                        {
                            if (operadorAnterior == '/' && numeroTemporal == "0")
                            {
                                historialDeOperaciones = $"{num1} / {numeroTemporal} =";
                                BorrarHistorial(coordOpY);
                                DibujarHistorial(historialDeOperaciones, coordOpX, coordOpY);
                                DibujarNumeros("ERROR", coordNumX, coordNumY);
                                Thread.Sleep(1000);
                                ReiniciarTodo(ref numeroTemporal, ref num1, ref num2, ref n1, ref n2, ref operadorAnterior,
                                    ref historialDeOperaciones, ref operacionTermianda, coordOpY, coordNumX, coordNumY);
                            }
                            else
                            {
                                res = RealizarOperacion(n1, n2, operadorAnterior);
                                resultado = res.ToString();
                                DarFormatoAlNumero(ref resultado, "Resultado");
                                historialDeOperaciones = $"{resultado} {teclaPulsada.KeyChar}";
                                DibujarNumeros(resultado, coordNumX, coordNumY);
                                num1 = resultado;
                                n1 = res;
                                num2 = "";
                                n2 = 0;
                                operadorAnterior = teclaPulsada.KeyChar;
                            }
                        }
                        BorrarHistorial(coordOpY);
                        DibujarHistorial(historialDeOperaciones, coordOpX, coordOpY);
                        numeroTemporal = "";
                    }
                }
                //Hace la operacion directamente
                else if (teclaPulsada.Key == ConsoleKey.Enter && (num1 != "" && numeroTemporal != "-" && numeroTemporal != "") && operacionTermianda == false)
                {
                    BorrarHistorial(coordOpY);
                    BorrarNumeros(coordNumY);
                    historialDeOperaciones = $"{num1} {operadorAnterior} {numeroTemporal} =";
                    DibujarHistorial(historialDeOperaciones, coordOpX, coordOpY);
                    //No se puede dividir entre 0
                    if (operadorAnterior == '/' && numeroTemporal == "0")
                    {
                        DibujarNumeros("ERROR", coordNumX, coordNumY);
                        Thread.Sleep(1000);
                        ReiniciarTodo(ref numeroTemporal, ref num1, ref num2, ref n1, ref n2, ref operadorAnterior,
                            ref historialDeOperaciones, ref operacionTermianda, coordOpY, coordNumX, coordNumY);
                    }
                    else
                    {
                        res = RealizarOperacion(n1, Convert.ToDouble(numeroTemporal), operadorAnterior);
                        resultado = res.ToString();
                        DarFormatoAlNumero(ref resultado, "Resultado");
                        DibujarNumeros(resultado, coordNumX, coordNumY);
                        /*num1 = resultado;
                        n1 = res;*/
                        num1 = "";
                        n1 = 0;
                        num2 = "";
                        n2 = 0;
                    }
                    numeroTemporal = resultado;
                    operacionTermianda = true;
                }
                //Detecta si quiere borrar un numero
                else if (teclaPulsada.Key == ConsoleKey.Backspace && numeroTemporal != "")
                {
                    if (operacionTermianda)
                    {
                        ReiniciarTodo(ref numeroTemporal, ref num1, ref num2, ref n1, ref n2, ref operadorAnterior,
                             ref historialDeOperaciones, ref operacionTermianda, coordOpY, coordNumX, coordNumY);
                    }
                    else
                    {
                        numeroTemporal = numeroTemporal.Substring(0, numeroTemporal.Length - 1);
                        if (numeroTemporal.EndsWith(",")) numeroTemporal = numeroTemporal.Substring(0, numeroTemporal.Length - 1);
                        BorrarNumeros(coordNumY);
                        if (numeroTemporal == "")
                        {
                            numeroTemporal = "0";
                        }
                        DibujarNumeros(numeroTemporal, coordNumX, coordNumY);
                    }
                }
                else if (teclaPulsada.Key == ConsoleKey.C)
                {
                    ReiniciarTodo(ref numeroTemporal, ref num1, ref num2, ref n1, ref n2, ref operadorAnterior,
                             ref historialDeOperaciones, ref operacionTermianda, coordOpY, coordNumX, coordNumY);
                }
            }
        }
        public static void ReiniciarTodo(ref string numTemporal, ref string num1, ref string num2,
            ref double n1, ref double n2, ref char operadorAnterior, ref string historial, ref bool operacionTerminada, int coordOpY, int coordNumX, int coordNumY)
        {
            numTemporal = "0";
            num1 = ""; num2 = "";
            n1 = 0; n2 = 0;
            operadorAnterior = '\0';
            historial = "";
            operacionTerminada = false;
            BorrarHistorial(coordOpY);
            BorrarNumeros(coordNumY);
            DibujarNumeros(numTemporal, coordNumX, coordNumY);
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
                case 'R':
                    char[,] R = {
                            {'█', '█', '█'},
                            {'█', ' ', '█'},
                            {'█', '█', ' '},
                            {'█', ' ', '█'},
                            {'█', ' ', '█'}
                        };
                    numero = R;
                    break;
                case 'O':
                    char[,] O = {
                            {'█', '█', '█'},
                            {'█', ' ', '█'},
                            {'█', ' ', '█'},
                            {'█', ' ', '█'},
                            {'█', '█', '█'}
                        };
                    numero = O;
                    break;
            }
            return numero;
        }
        public static void DibujarCaracter(char n, int x, int y)
        {
            char[,] caracter = ObtenerMatrizDelNumero(n);
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
        }
        public static void DibujarNumeros(string numeros, int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.White;
            //Establece la coordenada x según la longitud del numero
            foreach (char cosa in numeros)
            {
                if (cosa == '.')
                {
                    x -= 2;
                }
                else if (cosa == ',')
                {
                    x -= 3;
                }
                else
                {
                    x -= 4;
                }
            }
            //Recorre todos los numeros
            foreach (char n in numeros)
            {
                //Dibujar el numero actual
                if (n == 'E' || n == 'R' || n == 'O') Console.ForegroundColor = ConsoleColor.Red;
                DibujarCaracter(n, x, y);
                if (n == 'E' || n == 'R' || n == 'O') Console.ResetColor();
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
        public static void DibujarNumeros(string numeros, ConsoleColor color, int x, int y)
        {
            Console.ForegroundColor = color;
            //Establece la coordenada x según la longitud del numero
            foreach (char cosa in numeros)
            {
                if (cosa == '.')
                {
                    x -= 2;
                }
                else if (cosa == ',')
                {
                    x -= 3;
                }
                else
                {
                    x -= 4;
                }
            }
            //Recorre todos los numeros
            foreach (char n in numeros)
            {
                //Dibujar el numero actual
                if (n == 'E' || n == 'R' || n == 'O') Console.ForegroundColor = ConsoleColor.Red;
                DibujarCaracter(n, x, y);
                if (n == 'E' || n == 'R' || n == 'O') Console.ResetColor();
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
