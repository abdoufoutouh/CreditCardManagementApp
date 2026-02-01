<template>
  <div class="cards-section">
    <div class="section-header">
      <h2 class="section-title">My Credit Cards</h2>
    </div>
    <div class="cards-grid">
      <CreditCardPreview
        v-for="card in cards"
        :key="card.id"
        :card-type="card.cardType"
        :masked-number="card.maskedNumber"
        :cardholder-name="card.cardholderName"
        :expiration-date="card.expirationDate"
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
  }
});
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
}
</style>
