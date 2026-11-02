import { API_ERROR_FALLBACK } from '@/shared/constants/api';

export const toErrorMessage = (error: unknown): string => (error instanceof Error ? error.message : API_ERROR_FALLBACK);
