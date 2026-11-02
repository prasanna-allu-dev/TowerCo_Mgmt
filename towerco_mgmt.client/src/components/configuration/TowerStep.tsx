import { DEFAULT_NEW_SITE_STATUS, TOWER_LIMITS, TOWER_TYPES } from '@/constants/domain';
import FormField from './FormField';
import { bind } from './stepProps';
import type { StepProps } from './stepProps';

function TowerStep(props: StepProps) {
    const { errors } = props;
    const { minHeightMetres, maxHeightMetres, minTenants, maxTenants } = TOWER_LIMITS;
    return (
        <>
            <div className="form-grid">
                <FormField label="Tower type" error={errors.towerType}>
                    <select {...bind(props, 'towerType')}>
                        {TOWER_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
                    </select>
                </FormField>
                <FormField label="Height (metres)" error={errors.heightMetres} hint={`${minHeightMetres} to ${maxHeightMetres} m`}>
                    <input {...bind(props, 'heightMetres')} type="number" min={minHeightMetres} max={maxHeightMetres} />
                </FormField>
                <FormField label="Tenant slots (max tenants)" error={errors.maxTenants} hint={`Operators that can lease space, ${minTenants} to ${maxTenants}`}>
                    <input {...bind(props, 'maxTenants')} type="number" min={minTenants} max={maxTenants} />
                </FormField>
            </div>
            <p className="muted">New sites start in <strong>{DEFAULT_NEW_SITE_STATUS}</strong> status with no tenants. Equipment is added later (Phase 2).</p>
        </>
    );
}

export default TowerStep;
