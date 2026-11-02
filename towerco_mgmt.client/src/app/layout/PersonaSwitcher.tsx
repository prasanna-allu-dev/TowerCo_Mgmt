import { useNavigate } from 'react-router-dom';
import { PERSONAS, PERSONA_SWITCHER_LABEL } from '@/app/personas';
import { usePersona } from '@/app/usePersona';
import { PersonaList, PersonaTrigger } from './PersonaMenuParts';
import { PERSONA_ELEMENT_IDS, usePersonaMenu } from './usePersonaMenu';

function usePersonaSelection() {
    const { persona, setPersona } = usePersona();
    const navigate = useNavigate();
    const selectedIndex = Math.max(0, PERSONAS.findIndex(p => p.id === persona));
    const selectPersona = (index: number) => {
        setPersona(PERSONAS[index].id);
        navigate(PERSONAS[index].landingRoute);
    };
    return { persona, selectedIndex, selectPersona };
}

function PersonaSwitcher() {
    const { persona, selectedIndex, selectPersona } = usePersonaSelection();
    const menu = usePersonaMenu({ count: PERSONAS.length, selectedIndex, onSelect: selectPersona });
    return (
        <div className="persona-switcher" id={PERSONA_ELEMENT_IDS.root}>
            <div className="visually-hidden" id={PERSONA_ELEMENT_IDS.label}>{PERSONA_SWITCHER_LABEL}</div>
            <PersonaTrigger persona={PERSONAS[selectedIndex]} menu={menu} />
            {menu.isOpen && <PersonaList selectedId={persona} menu={menu} />}
        </div>
    );
}

export default PersonaSwitcher;
