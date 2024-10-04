import { openModal, closeModal } from '@/shared/ui/modalsManager'

import SectionFormModalLoader from './SectionFormModalLoader'

import type { Section } from '@/entities/section'

const modalId = 'sectionFormModal'

function openAddSectionFormModal() {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: 'Add new section',
      closeOnClickOutside: false,
      size: 400,
    },
    data: {
      children: <SectionFormModalLoader />,
    },
  })
}

function openEditSectionFormModal(section: Section) {
  openModal({
    modalId: modalId,
    name: 'content',
    settings: {
      title: `Edit section: #${section.id}`,
      closeOnClickOutside: false,
      size: 400,
    },
    data: {
      children: <SectionFormModalLoader section={section} />,
    },
  })
}

function closeSectionFormModal() {
  closeModal(modalId)
}

export { openAddSectionFormModal, openEditSectionFormModal, closeSectionFormModal }
