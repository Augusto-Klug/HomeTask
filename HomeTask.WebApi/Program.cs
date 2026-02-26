using HomeTask.Infrastructure;
using HomeTask.WebApi.Conversores.Implementacoes.Agendamento;
using HomeTask.WebApi.Conversores.Implementacoes.Avaliacao;
using HomeTask.WebApi.Conversores.Implementacoes.Cliente;
using HomeTask.WebApi.Conversores.Implementacoes.Mensagem;
using HomeTask.WebApi.Conversores.Implementacoes.Pagamento;
using HomeTask.WebApi.Conversores.Implementacoes.Prestador;
using HomeTask.WebApi.Conversores.Implementacoes.ServicoOferecido;
using HomeTask.WebApi.Conversores.Implementacoes.Usuario;
using HomeTask.WebApi.Conversores.Interfaces.Agendamento;
using HomeTask.WebApi.Conversores.Interfaces.Avaliacao;
using HomeTask.WebApi.Conversores.Interfaces.Cliente;
using HomeTask.WebApi.Conversores.Interfaces.Mensagem;
using HomeTask.WebApi.Conversores.Interfaces.Pagamento;
using HomeTask.WebApi.Conversores.Interfaces.Prestador;
using HomeTask.WebApi.Conversores.Interfaces.ServicoOferecido;
using HomeTask.WebApi.Conversores.Interfaces.Usuario;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IConversorCliente, ConversorCliente>();
builder.Services.AddScoped<IConversorUsuario, ConversorUsuario>();
builder.Services.AddScoped<IConversorPrestador, ConversorPrestador>();
builder.Services.AddScoped<IConversorAgendamento, ConversorAgendamento>();
builder.Services.AddScoped<IConversorAvaliacao, ConversorAvaliacao>();
builder.Services.AddScoped<IConversorPagamento, ConversorPagamento>();
builder.Services.AddScoped<IConversorMensagem, ConversorMensagem>();
builder.Services.AddScoped<IConversorServicoOferecido, ConversorServicoOferecido>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
