<template>
  <div class="search-section">
    <div class="search-container">
      <h3 class="search-title">Search Credit Cards</h3>
      
      <div class="search-filters">
        <div class="filter-group">
          <label for="cardType" class="filter-label">Card Type</label>
          <select 
            id="cardType"
            v-model="selectedCardType" 
            class="filter-select"
            @change="handleSearch"
          >
            <option value="">All Types</option>
            <option value="Visa">Visa</option>
            <option value="Mastercard">Mastercard</option>
            <option value="Amex">American Express</option>
          </select>
        </div>

        <div class="filter-group">
          <label for="cardNumber" class="filter-label">Card Number (Last 4 Digits)</label>
          <input 
            id="cardNumber"
            v-model="searchCardNumber" 
            type="text"
            placeholder="Enter last 4 digits"
            maxlength="4"
            class="filter-input"
            @input="handleSearch"
          />
        </div>

        <button 
          class="reset-btn"
          @click="resetSearch"
          :disabled="!hasActiveSearch"
        >
          Reset
        </button>
      </div>

      <div v-if="errorMessage" class="error-message">
        {{ errorMessage }}
      </div>

      <div v-if="isSearching" class="loading-state">
        <div class="loading-spinner"></div>
        <p>Searching...</p>
      </div>

      <div v-else-if="hasSearched && searchResults.length === 0" class="no-results">
        <p>No credit cards found matching your search criteria.</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useCreditCardAPI } from '../../composables/useCreditCardAPI';

const { searchCreditCards, isLoading: isSearching } = useCreditCardAPI();

const selectedCardType = ref('');
const searchCardNumber = ref('');
const searchResults = ref([]);
const errorMessage = ref('');
const hasSearched = ref(false);

const hasActiveSearch = computed(() => {
  return selectedCardType.value !== '' || searchCardNumber.value !== '';
});

const handleSearch = async () => {
  // Don't search if both fields are empty
  if (!selectedCardType.value && !searchCardNumber.value) {
    hasSearched.value = false;
    searchResults.value = [];
    errorMessage.value = '';
    return;
  }

  errorMessage.value = '';
  hasSearched.value = true;

  const result = await searchCreditCards(
    selectedCardType.value || null,
    searchCardNumber.value || null
  );

  if (result.success) {
    searchResults.value = result.data || [];
  } else {
    errorMessage.value = result.error || 'Search failed';
    searchResults.value = [];
  }

  // Emit results to parent
  emit('search-results', searchResults.value);
};

const resetSearch = () => {
  selectedCardType.value = '';
  searchCardNumber.value = '';
  searchResults.value = [];
  errorMessage.value = '';
  hasSearched.value = false;
  emit('search-results', []);
};

const emit = defineEmits(['search-results']);
</script>

<style scoped>
.search-section {
  margin-bottom: 30px;
  padding: 20px;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.search-container {
  max-width: 100%;
}

.search-title {
  margin: 0 0 20px 0;
  font-size: 18px;
  font-weight: 600;
  color: #2c3e50;
}

.search-filters {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
  align-items: flex-end;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
  flex: 1;
  min-width: 200px;
}

.filter-label {
  font-size: 14px;
  font-weight: 500;
  color: #2c3e50;
}

.filter-select,
.filter-input {
  padding: 10px 12px;
  border: 2px solid #dee2e6;
  border-radius: 6px;
  font-size: 14px;
  transition: all 0.3s ease;
  background-color: white;
}

.filter-select:hover,
.filter-input:hover {
  border-color: #667eea;
}

.filter-select:focus,
.filter-input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.filter-input {
  font-family: 'Courier New', monospace;
  letter-spacing: 2px;
}

.reset-btn {
  padding: 10px 20px;
  background-color: #6c757d;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.reset-btn:hover:not(:disabled) {
  background-color: #5a6268;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.reset-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.error-message {
  margin-top: 15px;
  padding: 12px;
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
  border-radius: 6px;
  font-size: 14px;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 30px 20px;
  text-align: center;
}

.loading-spinner {
  width: 30px;
  height: 30px;
  border: 3px solid #f3f3f3;
  border-top: 3px solid #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 10px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.loading-state p {
  color: #6c757d;
  font-size: 14px;
  margin: 0;
}

.no-results {
  margin-top: 15px;
  padding: 15px;
  background-color: #e2e3e5;
  color: #383d41;
  border: 1px solid #d6d8db;
  border-radius: 6px;
  font-size: 14px;
  text-align: center;
}

@media (max-width: 768px) {
  .search-filters {
    flex-direction: column;
  }

  .filter-group {
    min-width: 100%;
  }

  .reset-btn {
    width: 100%;
  }
}
</style>
