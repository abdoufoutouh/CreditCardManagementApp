<template>
  <div class="cards-section">
    <div class="section-header">
      <h2 class="section-title">My Credit Cards</h2>
    </div>
    
    <div v-if="loading" class="loading-state">
      <div class="loading-spinner"></div>
      <p>Loading your credit cards...</p>
    </div>
    
    <div v-else-if="cards.length === 0" class="empty-state">
      <div class="empty-icon">💳</div>
      <h3>No Credit Cards Found</h3>
      <p>You don't have any credit cards yet. Create your first one!</p>
    </div>
    
    <div v-else class="cards-grid">
      <CreditCardPreview
        v-for="card in cards"
        :key="card.id"
        :card-id="card.id"
        :card-type="card.cardType"
        :card-number="card.cardNumber"
        :cardholder-name="card.cardholderName"
        :expiration-date="card.expirationDate"
        :credit-limit="card.creditLimit"
        :current-balance="card.currentBalance"
        :show-balance="true"
        :show-actions="true"
        @show-details="$emit('show-details', $event)"
        @update="$emit('update', $event)"
        @delete="$emit('delete', $event)"
      />
    </div>
  </div>
</template>

<script setup>
import CreditCardPreview from '../cards/CreditCardPreview.vue';

defineProps({
  cards: {
    type: Array,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  }
});

defineEmits(['show-details', 'update', 'delete']);
</script>

<style scoped>
.cards-section {
  margin-bottom: 30px;
}

.section-header {
  margin-bottom: 25px;
}

.section-title {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: #2c3e50;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.loading-state p {
  color: #6c757d;
  font-size: 16px;
  margin: 0;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
  background: #f8f9fa;
  border-radius: 12px;
  border: 2px dashed #dee2e6;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 20px;
  opacity: 0.5;
}

.empty-state h3 {
  color: #2c3e50;
  font-size: 20px;
  font-weight: 600;
  margin: 0 0 10px 0;
}

.empty-state p {
  color: #6c757d;
  font-size: 16px;
  margin: 0;
  max-width: 400px;
}

.cards-grid {
  display: flex;
  gap: 25px;
  flex-wrap: wrap;
}

@media (max-width: 768px) {
  .cards-grid {
    flex-direction: column;
    align-items: center;
  }
  
  .empty-state {
    padding: 40px 20px;
  }
  
  .empty-icon {
    font-size: 36px;
  }
  
  .empty-state h3 {
    font-size: 18px;
  }
  
  .empty-state p {
    font-size: 14px;
  }
}
</style>
