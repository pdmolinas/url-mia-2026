using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    static void Main()
    {
        List<Estudiante> estudiantes = new List<Estudiante>();

        string[] lineas = File.ReadAllLines("estudiantes.csv");

        for (int i = 1; i < lineas.Length; i++)
        {
            string[] campos = lineas[i].Split(',');

            Estudiante estudiante = new Estudiante
            {
                Id = int.Parse(campos[0]),
                Nombre = campos[1],
                Carrera = campos[2]
            };

            estudiantes.Add(estudiante);
        }

        foreach (Estudiante e in estudiantes)
        {
            Console.WriteLine($"{e.Id}. - {e.Nombre} - {e.Carrera}");
        }

        var opciones = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(estudiantes, opciones);
        File.WriteAllText("estudiantes.json", json);

        Console.WriteLine("\nArchivo estudiantes.json creado correctamente.");
    }
}