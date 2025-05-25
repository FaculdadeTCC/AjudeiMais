//using AjudeiMais.API.Services;
//using AjudeiMais.Data.Models.PedidoModel;
//using Microsoft.AspNetCore.Mvc;

//namespace AjudeiMais.API.Controllers
//{
//    [Route("Api/[controller]")]
//    [ApiController]
//    public class PedidoController : ControllerBase
//    {
//        private readonly PedidoService _pedidoService;

//        public PedidoController(PedidoService pedidoService)
//        {
//            _pedidoService = pedidoService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetPedidos()
//        {
//            try
//            {
//                var pedidos = await _pedidoService.GetAll();
//                return Ok(pedidos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpGet("Ativos")]
//        public async Task<IActionResult> GetPedidosAtivos()
//        {
//            try
//            {
//                var pedidos = await _pedidoService.GetItens();
//                return Ok(pedidos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpGet("Instituição")]
//        public async Task<IActionResult> GetPedidosPorInstituicao(int Id)
//        {
//            try
//            {
//                var pedidos = await _pedidoService.GetByInstituicaoId(Id);
//                return Ok(pedidos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> SaveOrUpdate(Pedido model)
//        {
//            try
//            {
//                await _pedidoService.SaveOrUpdate(model);

//                if (model.Pedido_ID == 0)
//                {
//                    return CreatedAtAction(nameof(GetPedidos), new { id = model.Pedido_ID }, model);
//                }
//                else
//                {
//                    return Ok(model);
//                }
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            try
//            {
//                await _pedidoService.Delete(id);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }
//    }
//}
