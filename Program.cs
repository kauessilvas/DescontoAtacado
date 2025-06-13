Console.Clear();

var produtos = Catalogo.Produtos;
var itensVendidos = Venda.Itens;

var avaliador = new AvaliadorDescontos(new CalculadoraDescontoAtacado());
var descontos = avaliador.Avaliar(produtos, itensVendidos);

decimal subtotal = itensVendidos.Sum(i => i.Parcial);
decimal totalDescontos = descontos.Sum(d => d.Desconto);
decimal total = subtotal - totalDescontos;

Console.WriteLine("--- Desconto no Atacado ---\n");
Console.WriteLine("Descontos:");
foreach (var desconto in descontos)
{
    Console.WriteLine($"{desconto.Gtin,13}   {desconto.Desconto,12:C2}");
}

Console.WriteLine();
Console.WriteLine($"(+) Subtotal  = {subtotal,12:C2}");
Console.WriteLine($"(-) Descontos = {totalDescontos,12:C2}");
Console.WriteLine($"(=) Total     = {total,12:C2}");

Console.Write("\nPressione qualquer tecla para finalizar o programa...");
Console.ReadKey(true);

class Produto
{
    public long Gtin { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal PrecoVarejo { get; set; }
    public decimal? PrecoAtacado { get; set; }
    public int? QtdeAtacado { get; set; }
}

class ItemVenda
{
    public int Id { get; set; }
    public long Gtin { get; set; }
    public int Quantidade { get; set; }
    public decimal Parcial { get; set; }
}

class ItemDesconto
{
    public long Gtin { get; set; }
    public decimal Desconto { get; set; }
}

interface ICalculadoraDesconto
{
    decimal Calcular(Produto produto, int quantidade);
}

class CalculadoraDescontoAtacado : ICalculadoraDesconto
{
    public decimal Calcular(Produto produto, int quantidade)
    {
        if (produto.PrecoAtacado == null || produto.QtdeAtacado == null)
            return 0;

        if (quantidade < produto.QtdeAtacado)
            return 0;

        decimal precoVarejo = produto.PrecoVarejo;
        decimal precoAtacado = produto.PrecoAtacado.Value;

        return (precoVarejo - precoAtacado) * quantidade;
    }
}

class AvaliadorDescontos
{
    private readonly ICalculadoraDesconto _calculadora;

    public AvaliadorDescontos(ICalculadoraDesconto calculadora)
    {
        _calculadora = calculadora;
    }

    public List<ItemDesconto> Avaliar(List<Produto> produtos, List<ItemVenda> itens)
    {
        var descontos = new List<ItemDesconto>();

        foreach (var grupo in itens.GroupBy(i => i.Gtin))
        {
            var produto = produtos.Single(p => p.Gtin == grupo.Key);
            int quantidadeTotal = grupo.Sum(i => i.Quantidade);

            var desconto = _calculadora.Calcular(produto, quantidadeTotal);
            if (desconto > 0)
                descontos.Add(new ItemDesconto { Gtin = produto.Gtin, Desconto = desconto });
        }

        return descontos;
    }
}

static class Catalogo
{
    public static List<Produto> Produtos =>
        [
            new() { Gtin = 7891024110348, Descricao = "SABONETE OLEO DE ARGAN 90G PALMOLIVE", PrecoVarejo = 2.88m, PrecoAtacado = 2.51m, QtdeAtacado = 12 },
            new() { Gtin = 7891048038017, Descricao = "CHÁ DE CAMOMILA DR.OETKER", PrecoVarejo = 4.40m, PrecoAtacado = 4.37m, QtdeAtacado = 3 },
            new() { Gtin = 7896066334509, Descricao = "TORRADA TRADICIONAL WICKBOLD PACOTE", PrecoVarejo = 5.19m },
            new() { Gtin = 7891700203142, Descricao = "BEBIDA À BASE DE SOJA MAÇÃ ADES CAIXA 200ML", PrecoVarejo = 2.39m, PrecoAtacado = 2.38m, QtdeAtacado = 6 },
            new() { Gtin = 7894321711263, Descricao = "ACHOCOLATADO TODDY 400G", PrecoVarejo = 9.79m },
            new() { Gtin = 7896001250611, Descricao = "ADOÇANTE SUCRALOSE LINEA 25ML", PrecoVarejo = 9.89m, PrecoAtacado = 9.10m, QtdeAtacado = 10 },
            new() { Gtin = 7793306013029, Descricao = "CEREAL SUCRILHOS 320G", PrecoVarejo = 12.79m, PrecoAtacado = 12.35m, QtdeAtacado = 3 },
            new() { Gtin = 7896004400914, Descricao = "COCO RALADO SOCOCO 50G", PrecoVarejo = 4.20m, PrecoAtacado = 4.05m, QtdeAtacado = 6 },
            new() { Gtin = 7898080640017, Descricao = "LEITE INTEGRAL ITALAC 1L", PrecoVarejo = 6.99m, PrecoAtacado = 6.89m, QtdeAtacado = 12 },
            new() { Gtin = 7891025301516, Descricao = "DANONINHO MORANGO 360G", PrecoVarejo = 12.99m },
            new() { Gtin = 7891030003115, Descricao = "CREME DE LEITE MOCOCA 200G", PrecoVarejo = 3.12m, PrecoAtacado = 3.09m, QtdeAtacado = 4 },
        ];
}

static class Venda
{
    public static List<ItemVenda> Itens =>
    [
        new() { Id = 1, Gtin = 7891048038017, Quantidade = 1, Parcial = 4.40m },
            new() { Id = 2, Gtin = 7896004400914, Quantidade = 4, Parcial = 16.80m },
            new() { Id = 3, Gtin = 7891030003115, Quantidade = 1, Parcial = 3.12m },
            new() { Id = 4, Gtin = 7891024110348, Quantidade = 6, Parcial = 17.28m },
            new() { Id = 5, Gtin = 7898080640017, Quantidade = 24, Parcial = 167.76m },
            new() { Id = 6, Gtin = 7896004400914, Quantidade = 8, Parcial = 33.60m },
            new() { Id = 7, Gtin = 7891700203142, Quantidade = 8, Parcial = 19.12m },
            new() { Id = 8, Gtin = 7891048038017, Quantidade = 1, Parcial = 4.40m },
            new() { Id = 9, Gtin = 7793306013029, Quantidade = 3, Parcial = 38.37m },
            new() { Id = 10, Gtin = 7896066334509, Quantidade = 2, Parcial = 10.38m },
        ];
}