namespace TestGraph.Analysis.Samples;

public static class AcademicSampleCatalog
{
    public static IReadOnlyList<AcademicSample> All { get; } =
    [
        new(
            "matriz-menor-par",
            "Matrix Minimum Even",
            "Angel Emmanuel Gonzalez Acosta",
            "Determina la columna que contiene el menor número par en una matriz 5x3.",
            """
            # Muestra académica preservada.
            # Requiere sintaxis de matrices/arreglos que no forma parte de TGPL V1.
            """,
            null,
            false,
            "Requires array/matrix syntax planned beyond TGPL V1."),

        new(
            "becas",
            "Scholarship Calculator",
            "Francis Jairo Matias Rosario",
            "Calcula el monto de una beca según edad y promedio.",
            """
            Entero edad
            Real promedio
            Real beca

            Leer edad
            Leer promedio

            Si edad > 18 Entonces
                Si promedio >= 9 Entonces
                    beca <- 2000
                Sino Si promedio >= 7.5 Entonces
                    beca <- 1000
                Sino Si promedio >= 6 Entonces
                    beca <- 500
                Sino
                    beca <- 0
                Fin Si
            Sino
                Si promedio >= 9 Entonces
                    beca <- 3000
                Sino Si promedio >= 8 Entonces
                    beca <- 2000
                Sino Si promedio >= 6 Entonces
                    beca <- 100
                Sino
                    beca <- 0
                Fin Si
            Fin Si

            Escribir beca
            """,
            8,
            true,
            "The original academic document reports V(G)=7, but the preserved TGPL flow contains seven binary decision nodes; TestGraph deterministically computes V(G)=8."),

        new(
            "terminal-4",
            "Numbers Ending in Four",
            "Robinson Junior Novo Lopez",
            "Muestra números terminados en 4 dentro de un rango.",
            """
            Entero inicio
            Entero fin
            Entero numero

            Leer inicio
            Leer fin

            Si inicio > fin Entonces
                numero <- inicio
                inicio <- fin
                fin <- numero
            Fin Si

            numero <- inicio
            Mientras numero <= fin Hacer
                Si numero % 10 = 4 Entonces
                    Escribir numero
                Fin Si
                numero <- numero + 1
            Fin Mientras
            """,
            4,
            true,
            "The original academic document contains inconsistent complexity calculations; TestGraph uses the CFG-derived result."),

        new(
            "matriz-24",
            "Find Number 24",
            "Christian Rainel Menendez Hiciano",
            "Busca el número 24 dentro de una matriz 4x3.",
            """
            # Muestra académica preservada.
            # Requiere sintaxis de matrices/arreglos que no forma parte de TGPL V1.
            """,
            7,
            false,
            "Requires array/matrix syntax planned beyond TGPL V1."),

        new(
            "costo-descuento",
            "Discount Calculator",
            "Diego Jose Montero Almonte",
            "Calcula un descuento de 15%, 12% o 10% según el precio.",
            """
            Real precio
            Real descuento

            Leer precio

            Si precio >= 200 Entonces
                descuento <- precio * 0.15
            Sino Si precio < 100 Entonces
                descuento <- precio * 0.10
            Sino
                descuento <- precio * 0.12
            Fin Si

            Escribir descuento
            """,
            3,
            true)
    ];

    public static AcademicSample? Find(string id) =>
        All.FirstOrDefault(sample => string.Equals(sample.Id, id, StringComparison.OrdinalIgnoreCase));
}
