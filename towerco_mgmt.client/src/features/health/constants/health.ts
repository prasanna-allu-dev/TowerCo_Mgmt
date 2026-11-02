import type { Severity } from '@/shared/types/domain';

export const HEALTH_STATUSES = ['Healthy', 'Warning', 'Critical'] as const;
export const SENSOR_TYPES = ['BatteryVoltage', 'ShelterTemperature', 'GeneratorFuelLevel', 'TowerTilt'] as const;

export const HEALTH_SEVERITY: Record<(typeof HEALTH_STATUSES)[number], Severity> = {
    Healthy: 'ok', Warning: 'warning', Critical: 'critical',
};

export const SENSOR_LABELS: Record<(typeof SENSOR_TYPES)[number], { label: string; unit: string }> = {
    BatteryVoltage: { label: 'Battery voltage', unit: 'V' },
    ShelterTemperature: { label: 'Shelter temperature', unit: '°C' },
    GeneratorFuelLevel: { label: 'Generator fuel', unit: '%' },
    TowerTilt: { label: 'Tower tilt', unit: '°' },
};

export const INSPECTION_OUTCOME_LABELS = { Pass: 'Pass', PassWithActions: 'Pass with actions', Fail: 'Fail' } as const;

export const SPARKLINE = { width: 220, height: 48, padding: 4 } as const;
