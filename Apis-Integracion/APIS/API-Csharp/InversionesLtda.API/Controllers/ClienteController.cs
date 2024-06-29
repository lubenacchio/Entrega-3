using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using InversionesLtda.API.Models;
using Microsoft.Extensions.Logging;

namespace InversionesLtda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private static IList<Cliente> lista = new List<Cliente>();
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(ILogger<ClienteController> logger)
        {
            _logger = logger;
        }

        // GET: api/Cliente
        [HttpGet]
        public ActionResult<IEnumerable<Cliente>> Get()
        {
            _logger.LogInformation("Solicitud GET recibida para obtener todos los clientes");
            return Ok(lista);
        }

        // GET api/Cliente/5
        [HttpGet("{id}")]
        public ActionResult<Cliente> Get(int id)
        {
            _logger.LogInformation($"Solicitud GET recibida para obtener cliente con id {id}");
            var cliente = lista.FirstOrDefault(x => x.id == id);

            if (cliente == null)
            {
                _logger.LogError($"Cliente con id {id} no encontrado");
                return NotFound(new { error = "Cliente no encontrado" });
            }

            return Ok(cliente);
        }

        // POST api/Cliente
        [HttpPost]
        public ActionResult Post([FromBody] Cliente value)
        {
            if (!TryValidateModel(value))
            {
                _logger.LogError("Datos del cliente inválidos en la solicitud POST");
                return BadRequest(ModelState);
            }

            lista.Add(value);
            _logger.LogInformation($"Cliente agregado con éxito: {value.id}");
            return Ok(new { message = "Cliente agregado con éxito" });
        }

        // PUT api/Cliente/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Cliente value)
        {
            if (!TryValidateModel(value))
            {
                _logger.LogError("Datos del cliente inválidos en la solicitud PUT");
                return BadRequest(ModelState);
            }

            var cliente = lista.FirstOrDefault(x => x.id == id);
            if (cliente == null)
            {
                _logger.LogError($"Cliente con id {id} no encontrado");
                return NotFound(new { error = "Cliente no encontrado" });
            }

            lista[lista.IndexOf(cliente)] = value;
            _logger.LogInformation($"Cliente con id {id} actualizado con éxito");
            return Ok(new { message = "Cliente actualizado con éxito" });
        }

        // DELETE api/Cliente/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var cliente = lista.FirstOrDefault(x => x.id == id);
            if (cliente == null)
            {
                _logger.LogError($"Cliente con id {id} no encontrado");
                return NotFound(new { error = "Cliente no encontrado" });
            }

            lista.Remove(cliente);
            _logger.LogInformation($"Cliente con id {id} eliminado con éxito");
            return Ok(new { message = "Cliente eliminado con éxito" });
        }
    }
}
