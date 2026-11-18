/** Keep in sync with Vite proxy target and API launchSettings (port 5080). */
export const API_PORT = 5080

export const PARSE_ENDPOINT = '/api/v1/parse'

export const STALE_DEBOUNCE_MS = 300

export const ClientErrorCode = {
  Unknown: 'UNKNOWN_ERROR',
  Network: 'NETWORK_ERROR',
} as const
