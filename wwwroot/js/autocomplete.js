// Componente de Autocomplete para selectores
class AutocompleteSelector {
    constructor(inputElement, hiddenInput, searchUrl, placeholder = "Buscar...") {
        this.inputElement = inputElement;
        this.hiddenInput = hiddenInput;
        this.searchUrl = searchUrl;
        this.placeholder = placeholder;
        this.dropdown = null;
        this.selectedItem = null;
        this.searchTimeout = null;
        
        this.init();
    }
    
    init() {
        // Configurar el input
        this.inputElement.placeholder = this.placeholder;
        this.inputElement.autocomplete = "off";
        
        // Crear el dropdown
        this.createDropdown();
        
        // Event listeners
        this.inputElement.addEventListener('input', (e) => this.handleInput(e));
        this.inputElement.addEventListener('keydown', (e) => this.handleKeydown(e));
        this.inputElement.addEventListener('blur', (e) => this.handleBlur(e));
        this.inputElement.addEventListener('focus', (e) => this.handleFocus(e));
        
        // Ocultar dropdown al hacer clic fuera
        document.addEventListener('click', (e) => {
            if (!this.inputElement.contains(e.target) && !this.dropdown.contains(e.target)) {
                this.hideDropdown();
            }
        });
    }
    
    createDropdown() {
        this.dropdown = document.createElement('div');
        this.dropdown.className = 'autocomplete-dropdown';
        this.dropdown.style.cssText = `
            position: absolute;
            top: 100%;
            left: 0;
            right: 0;
            background: white;
            border: 1px solid #ccc;
            border-top: none;
            max-height: 200px;
            overflow-y: auto;
            z-index: 1000;
            display: none;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        `;
        
        // Insertar el dropdown después del input
        this.inputElement.parentNode.style.position = 'relative';
        this.inputElement.parentNode.appendChild(this.dropdown);
    }
    
    handleInput(e) {
        const query = e.target.value.trim();
        
        // Limpiar timeout anterior
        if (this.searchTimeout) {
            clearTimeout(this.searchTimeout);
        }
        
        // Si está vacío, limpiar selección
        if (query === '') {
            this.clearSelection();
            this.hideDropdown();
            return;
        }
        
        // Debounce la búsqueda
        this.searchTimeout = setTimeout(() => {
            this.search(query);
        }, 300);
    }
    
    handleKeydown(e) {
        const items = this.dropdown.querySelectorAll('.autocomplete-item');
        const currentIndex = Array.from(items).findIndex(item => item.classList.contains('active'));
        
        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                this.navigateItems(items, currentIndex + 1);
                break;
            case 'ArrowUp':
                e.preventDefault();
                this.navigateItems(items, currentIndex - 1);
                break;
            case 'Enter':
                e.preventDefault();
                if (currentIndex >= 0 && items[currentIndex]) {
                    this.selectItem(items[currentIndex]);
                }
                break;
            case 'Escape':
                this.hideDropdown();
                break;
        }
    }
    
    handleBlur(e) {
        // Delay para permitir clics en el dropdown
        setTimeout(() => {
            if (!this.dropdown.contains(document.activeElement)) {
                this.hideDropdown();
            }
        }, 150);
    }
    
    handleFocus(e) {
        if (this.inputElement.value.trim() !== '') {
            this.search(this.inputElement.value.trim());
        }
    }
    
    navigateItems(items, newIndex) {
        // Remover clase active de todos los items
        items.forEach(item => item.classList.remove('active'));
        
        // Aplicar clase active al item correcto
        if (newIndex >= 0 && newIndex < items.length) {
            items[newIndex].classList.add('active');
            items[newIndex].scrollIntoView({ block: 'nearest' });
        }
    }
    
    async search(query) {
        try {
            const response = await fetch(`${this.searchUrl}?term=${encodeURIComponent(query)}`);
            const data = await response.json();
            
            this.showResults(data);
        } catch (error) {
            console.error('Error en búsqueda:', error);
            this.hideDropdown();
        }
    }
    
    showResults(results) {
        this.dropdown.innerHTML = '';
        
        if (results.length === 0) {
            const noResults = document.createElement('div');
            noResults.className = 'autocomplete-item no-results';
            noResults.textContent = 'No se encontraron resultados';
            noResults.style.cssText = 'padding: 10px; color: #666; font-style: italic;';
            this.dropdown.appendChild(noResults);
        } else {
            results.forEach((item, index) => {
                const itemElement = document.createElement('div');
                itemElement.className = 'autocomplete-item';
                itemElement.textContent = item.text;
                itemElement.dataset.id = item.id;
                itemElement.style.cssText = `
                    padding: 10px;
                    cursor: pointer;
                    border-bottom: 1px solid #eee;
                `;
                
                // Hover effect
                itemElement.addEventListener('mouseenter', () => {
                    itemElement.style.backgroundColor = '#f5f5f5';
                });
                itemElement.addEventListener('mouseleave', () => {
                    itemElement.style.backgroundColor = '';
                });
                
                // Click handler
                itemElement.addEventListener('click', () => {
                    this.selectItem(itemElement);
                });
                
                this.dropdown.appendChild(itemElement);
            });
        }
        
        this.showDropdown();
    }
    
    selectItem(itemElement) {
        if (itemElement.classList.contains('no-results')) return;
        
        const id = itemElement.dataset.id;
        const text = itemElement.textContent;
        
        // Actualizar inputs
        this.inputElement.value = text;
        this.hiddenInput.value = id;
        
        // Guardar selección
        this.selectedItem = { id, text };
        
        // Ocultar dropdown
        this.hideDropdown();
        
        // Trigger change event
        this.hiddenInput.dispatchEvent(new Event('change'));
    }
    
    clearSelection() {
        this.inputElement.value = '';
        this.hiddenInput.value = '';
        this.selectedItem = null;
    }
    
    showDropdown() {
        this.dropdown.style.display = 'block';
    }
    
    hideDropdown() {
        this.dropdown.style.display = 'none';
    }
    
    // Método para establecer valor programáticamente
    setValue(id, text) {
        this.inputElement.value = text;
        this.hiddenInput.value = id;
        this.selectedItem = { id, text };
    }
}

// Función helper para inicializar autocomplete
function initAutocomplete(inputId, hiddenInputId, searchUrl, placeholder) {
    const inputElement = document.getElementById(inputId);
    const hiddenInput = document.getElementById(hiddenInputId);
    
    if (inputElement && hiddenInput) {
        return new AutocompleteSelector(inputElement, hiddenInput, searchUrl, placeholder);
    }
    
    return null;
}
