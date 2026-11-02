import {
    LATITUDE_LIMIT, LONGITUDE_LIMIT, SITE_CODE_EXAMPLE, SITE_CODE_PATTERN, TOWER_LIMITS, TOWER_TYPES,
} from '@/shared/constants/domain';
import type { CreateSiteRequest } from '@/features/sites/types';

export type FormField = keyof CreateSiteRequest;
export type FormValues = Record<FormField, string>;
export type FormErrors = Partial<Record<FormField, string>>;

export const WIZARD_STEPS = ['Site details', 'Tower specification', 'Review and create'] as const;
export type WizardStep = 0 | 1 | 2;
export const LAST_STEP: WizardStep = 2;

export const EMPTY_FORM: FormValues = {
    siteCode: '', name: '', countryCode: '', city: '', latitude: '', longitude: '',
    towerType: TOWER_TYPES[0], heightMetres: '', maxTenants: '',
};

const REQUIRED = 'Required.';

function requireAll(values: FormValues, fields: FormField[]): FormErrors {
    return Object.fromEntries(fields.filter(f => !values[f].trim()).map(f => [f, REQUIRED]));
}

function outOfRange(value: string, min: number, max: number): boolean {
    const n = Number(value);
    return value !== '' && (Number.isNaN(n) || n < min || n > max);
}

export function validateSiteStep(values: FormValues): FormErrors {
    const errors = requireAll(values, ['siteCode', 'name', 'countryCode', 'city', 'latitude', 'longitude']);
    if (values.siteCode && !SITE_CODE_PATTERN.test(values.siteCode)) errors.siteCode = `Use format ${SITE_CODE_EXAMPLE}.`;
    if (outOfRange(values.latitude, -LATITUDE_LIMIT, LATITUDE_LIMIT)) errors.latitude = `Between -${LATITUDE_LIMIT} and ${LATITUDE_LIMIT}.`;
    if (outOfRange(values.longitude, -LONGITUDE_LIMIT, LONGITUDE_LIMIT)) errors.longitude = `Between -${LONGITUDE_LIMIT} and ${LONGITUDE_LIMIT}.`;
    return errors;
}

export function validateTowerStep(values: FormValues): FormErrors {
    const { minHeightMetres, maxHeightMetres, minTenants, maxTenants } = TOWER_LIMITS;
    const errors = requireAll(values, ['towerType', 'heightMetres', 'maxTenants']);
    if (outOfRange(values.heightMetres, minHeightMetres, maxHeightMetres)) errors.heightMetres = `Between ${minHeightMetres} and ${maxHeightMetres} m.`;
    if (outOfRange(values.maxTenants, minTenants, maxTenants)) errors.maxTenants = `Between ${minTenants} and ${maxTenants}.`;
    return errors;
}

export const STEP_VALIDATORS: Record<WizardStep, (values: FormValues) => FormErrors> = {
    0: validateSiteStep,
    1: validateTowerStep,
    2: values => ({ ...validateSiteStep(values), ...validateTowerStep(values) }),
};

export function toRequest(values: FormValues): CreateSiteRequest {
    return {
        siteCode: values.siteCode.trim().toUpperCase(), name: values.name.trim(),
        countryCode: values.countryCode, city: values.city.trim(),
        latitude: Number(values.latitude), longitude: Number(values.longitude),
        towerType: values.towerType, heightMetres: Number(values.heightMetres), maxTenants: Number(values.maxTenants),
    };
}
