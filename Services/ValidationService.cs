using Inmobiliaria.Models;
using Inmobiliaria.Data;

namespace Inmobiliaria.Services
{
    public class ValidationService
    {
        private readonly DatabaseConnection _dbConnection;

        public ValidationService(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Verifica si un propietario puede ser eliminado
        /// </summary>
        /// <param name="propietarioId">ID del propietario a verificar</param>
        /// <returns>Resultado de la validación con mensaje de error si aplica</returns>
        public ValidationResult CanDeletePropietario(int propietarioId)
        {
            var result = new ValidationResult { IsValid = true };

            // Verificar si tiene inmuebles con contratos vigentes
            var query = @"
                SELECT COUNT(*) 
                FROM Contratos c
                INNER JOIN Inmuebles i ON c.InmuebleId = i.Id
                WHERE i.PropietarioId = @propietarioId 
                AND c.FechaTerminacionAnticipada IS NULL 
                AND c.FechaFin >= date('now')";

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = connection.CreateCommand();
            command.CommandText = query;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@propietarioId";
            parameter.Value = propietarioId;
            command.Parameters.Add(parameter);
            
            var contratosVigentes = Convert.ToInt32(command.ExecuteScalar());

            if (contratosVigentes > 0)
            {
                result.IsValid = false;
                result.ErrorMessage = $"No se puede eliminar el propietario porque tiene {contratosVigentes} contrato(s) vigente(s). " +
                                    "Debe terminar o renovar los contratos antes de eliminar el propietario.";
            }

            return result;
        }

        /// <summary>
        /// Verifica si un inquilino puede ser eliminado
        /// </summary>
        /// <param name="inquilinoId">ID del inquilino a verificar</param>
        /// <returns>Resultado de la validación con mensaje de error si aplica</returns>
        public ValidationResult CanDeleteInquilino(int inquilinoId)
        {
            var result = new ValidationResult { IsValid = true };

            // Verificar si tiene contratos vigentes
            var query = @"
                SELECT COUNT(*) 
                FROM Contratos 
                WHERE InquilinoId = @inquilinoId 
                AND FechaTerminacionAnticipada IS NULL 
                AND FechaFin >= date('now')";

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = connection.CreateCommand();
            command.CommandText = query;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@inquilinoId";
            parameter.Value = inquilinoId;
            command.Parameters.Add(parameter);
            
            var contratosVigentes = Convert.ToInt32(command.ExecuteScalar());

            if (contratosVigentes > 0)
            {
                result.IsValid = false;
                result.ErrorMessage = $"No se puede eliminar el inquilino porque tiene {contratosVigentes} contrato(s) vigente(s). " +
                                    "Debe terminar los contratos antes de eliminar el inquilino.";
            }

            return result;
        }

        /// <summary>
        /// Obtiene los contratos vigentes de un propietario
        /// </summary>
        /// <param name="propietarioId">ID del propietario</param>
        /// <returns>Lista de contratos vigentes</returns>
        public List<ContratoInfo> GetContratosVigentesPropietario(int propietarioId)
        {
            var contratos = new List<ContratoInfo>();
            var query = @"
                SELECT c.Id, c.FechaInicio, c.FechaFin, i.Direccion, 
                       inq.Nombre || ' ' || inq.Apellido as InquilinoNombre
                FROM Contratos c
                INNER JOIN Inmuebles i ON c.InmuebleId = i.Id
                INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id
                WHERE i.PropietarioId = @propietarioId 
                AND c.FechaTerminacionAnticipada IS NULL 
                AND c.FechaFin >= date('now')
                ORDER BY c.FechaFin";

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = connection.CreateCommand();
            command.CommandText = query;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@propietarioId";
            parameter.Value = propietarioId;
            command.Parameters.Add(parameter);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                contratos.Add(new ContratoInfo
                {
                    Id = reader.GetInt32("Id"),
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    DireccionInmueble = reader.GetString("Direccion"),
                    InquilinoNombre = reader.GetString("InquilinoNombre")
                });
            }

            return contratos;
        }

        /// <summary>
        /// Obtiene los contratos vigentes de un inquilino
        /// </summary>
        /// <param name="inquilinoId">ID del inquilino</param>
        /// <returns>Lista de contratos vigentes</returns>
        public List<ContratoInfo> GetContratosVigentesInquilino(int inquilinoId)
        {
            var contratos = new List<ContratoInfo>();
            var query = @"
                SELECT c.Id, c.FechaInicio, c.FechaFin, i.Direccion, 
                       p.Nombre || ' ' || p.Apellido as PropietarioNombre
                FROM Contratos c
                INNER JOIN Inmuebles i ON c.InmuebleId = i.Id
                INNER JOIN Propietarios p ON i.PropietarioId = p.Id
                WHERE c.InquilinoId = @inquilinoId 
                AND c.FechaTerminacionAnticipada IS NULL 
                AND c.FechaFin >= date('now')
                ORDER BY c.FechaFin";

            using var connection = _dbConnection.GetConnection();
            connection.Open();
            
            using var command = connection.CreateCommand();
            command.CommandText = query;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@inquilinoId";
            parameter.Value = inquilinoId;
            command.Parameters.Add(parameter);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                contratos.Add(new ContratoInfo
                {
                    Id = reader.GetInt32("Id"),
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    DireccionInmueble = reader.GetString("Direccion"),
                    PropietarioNombre = reader.GetString("PropietarioNombre")
                });
            }

            return contratos;
        }
    }

    /// <summary>
    /// Resultado de una validación
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Información básica de un contrato para mostrar en validaciones
    /// </summary>
    public class ContratoInfo
    {
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string DireccionInmueble { get; set; } = string.Empty;
        public string InquilinoNombre { get; set; } = string.Empty;
        public string PropietarioNombre { get; set; } = string.Empty;
    }
}