'*******************************************************************************'
'En este módulo se almacenan todas las enumeraciones usadas por el sistema, 
'en ninguna otra parte del código deben existir definiciones de enumeraciones
'más que en este módulo.
'*******************************************************************************' 
#Region "Enumeraciones"

''' -----------------------------------------------------------------------------
''' <summary>
''' Tipos de empresa existentes
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[CENCLOPEZB]	8/12/2005	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Enum TipoEmpresa
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de empresa oficial
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/12/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Oficial = 1
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Tipo de empresa privada
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCLOPEZB]	8/12/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Privada = 2
 
    'todo: revisar bien esta programacion, este tipo no deberia existir, afecta a la consulta 
    'de listado de facturas
    Desconocida = 0
End Enum
''' -----------------------------------------------------------------------------
''' <summary>
''' Sirve para modelar los tipos de afiliaciones que se pueden realizar mediante
''' TRANSAR
''' </summary>
''' <remarks>
'''     Caja = 0
'''     POS = 1
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	12/23/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Enum TipoAfiliacion
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indica una afiliación a Caja de Compensación Familiar
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	1/18/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Caja = 0
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indica una afiliación a EPS
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	1/18/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    POS = 1
End Enum
''' -----------------------------------------------------------------------------
''' <summary>
''' Modela el tipo de canal usado para realizar una afiliación.
''' </summary>
''' <remarks>
'''     Transar = 0
'''     Otro = 1
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	12/23/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Enum TipoCanal
    Transar = 0
    Otro = 1
End Enum
''' -----------------------------------------------------------------------------
''' <summary>
''' Modela los tipos  de cambios o novedades que se pueden realizar sobre trabajadores
''' o beneficiarios de caja de compensación familiar.
''' </summary>
''' <remarks>
'''     Retiro = 0
'''     Modificacion = 1
''' </remarks>
''' <history>
''' 	[CENCXLOPEZC]	12/23/2004	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Enum TipoNovedad
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indica que la novedad es un retiro 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	1/18/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Retiro = 0
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indica que la novedad es una modificación de datos de un individuo que ya 
    ''' existe en la base de datos de compensar.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[CENCXLOPEZC]	1/18/2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Modificacion = 1
End Enum


#End Region

