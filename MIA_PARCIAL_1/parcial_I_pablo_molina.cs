using System;
using System.IO;

public class ResultadoEstudiante
{
    public string NombreApellido { get; set; } = string.Empty;
    public int Lineas { get; set; }
    public int Palabras { get; set; }
    public int Caracteres { get; set; }

    public string ToCsvLine()
    {
        return $"{NombreApellido},{Lineas},{Palabras},{Caracteres}";
    }
}

class parcial_I_pablo_molina
{
    static void Main(string[] args)
    {
        Console.Write("Ingrese su nombre completo: ");
        string nombreCompleto = Console.ReadLine();

        string nombreFormateado = nombreCompleto.Trim().Replace(" ", "_");

        Console.Write("Ingrese la ruta del archivo: ");
        string rutaArchivo = Console.ReadLine();

        string[] lineas = File.ReadAllLines(rutaArchivo);
        string contenido = File.ReadAllText(rutaArchivo);

        int numLineas = lineas.Length;
        int numPalabras = contenido.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        int numCaracteres = contenido.Length;

        ResultadoEstudiante resultado = new ResultadoEstudiante
        {
            NombreApellido = nombreFormateado,
            Lineas = numLineas,
            Palabras = numPalabras,
            Caracteres = numCaracteres
        };

        string directorio = Path.GetDirectoryName(rutaArchivo);
        string rutaCsv = directorio + @"\resultados_" + nombreFormateado + ".csv";

        string contenidoCsv = $"Nombre_Apellido,Lineas,Palabras,Caracteres\n{resultado.ToCsvLine()}\n";
        File.WriteAllText(rutaCsv, contenidoCsv);

        Console.WriteLine();
        Console.WriteLine($"Usuario: {nombreCompleto}");
        Console.WriteLine($"Archivo: {rutaArchivo}");
        Console.WriteLine($"El archivo contiene: {resultado.Lineas} lineas, {resultado.Palabras} palabras, {resultado.Caracteres} caracteres.");
        Console.WriteLine($"Resultados guardados en {rutaCsv}");
    }
}
