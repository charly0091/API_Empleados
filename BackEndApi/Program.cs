using BackEndApi.Models;
using Microsoft.EntityFrameworkCore;

using BackEndApi.Services.Contrato;
using BackEndApi.Services.Implementacion;

using AutoMapper;
using BackEndApi.DTOs;
using BackEndApi.Utilidades;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbempleadoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region peticiones API REST
app.MapGet("/departamento/lista", async (
    IDepartamentoService _departamentoServicio,
    IMapper _mapper) =>
{
    List<Departamento> listaDepartamento = await _departamentoServicio.GetList();
    List<DepartamentoDTO> listaDepartamentoDTO = _mapper.Map<List<DepartamentoDTO>>(listaDepartamento);

    if(listaDepartamentoDTO.Count > 0)
         return Results.Ok(listaDepartamentoDTO);
    else
        return Results.NotFound();

});

app.MapGet("/empleado/lista", async (
    IEmpleadoService _empleadoservicio,
    IMapper _mapper) =>
{
    List<Empleado> listaEmpleado = await _empleadoservicio.GetList();
    List<EmpleadoDTO> listaEmpleadoDTO = _mapper.Map<List<EmpleadoDTO>>(listaEmpleado);

    if (listaEmpleadoDTO.Count > 0)
        return Results.Ok(listaEmpleadoDTO);
    else
        return Results.NotFound();

});

app.MapPost("/empleado/guardar", async (
    EmpleadoDTO modelo,
    IEmpleadoService _empleadoservicio,
    IMapper _mapper
    ) => {  
        Empleado _empleado = _mapper.Map<Empleado>(modelo);
        var _empleadoCreado = await _empleadoservicio.Add(_empleado);

        if(_empleadoCreado.IdEmpleado != 0)
            return Results.Ok(_mapper.Map<EmpleadoDTO>(_empleadoCreado));
        else
            return Results.StatusCode(StatusCodes.Status500InternalServerError);


});


app.MapPost("/empleado/actualizar/{IdEmpleado}", async (
        int IdEmpleado,
        EmpleadoDTO modelo,
        IEmpleadoService _empleadoservicio,
        IMapper _mapper
    ) => {
        
        var _encontrado = await _empleadoservicio.Get(IdEmpleado);
        if(_encontrado is null)
            return Results.NotFound();

        var _empleado = _mapper.Map<Empleado>(modelo);
        _encontrado.NombreCompleto = _empleado.NombreCompleto;
        _encontrado.IdDepartamento = _empleado.IdDepartamento;
        _encontrado.Sueldo = _empleado.Sueldo;
        _encontrado.FechaContrato = _empleado.FechaContrato;

        var respuesta = await _empleadoservicio.Update(_encontrado);

        if(respuesta)
            return Results.Ok(_mapper.Map<EmpleadoDTO>(_encontrado));
        else
            return Results.StatusCode(StatusCodes.Status500InternalServerError);


    });


app.MapPost("/empleado/eliminar/{IdEmpleado}", async (
    int IdEmpleado,
    IEmpleadoService _empleadoservicio
    ) => {

        var _encontrado = await _empleadoservicio.Get(IdEmpleado);

        if (_encontrado is null)
            return Results.NotFound();

        var respuesta = await _empleadoservicio.Delete(_encontrado);

        if (respuesta)
            return Results.Ok();
        else
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
    });

#endregion peticiones API REST


app.Run();